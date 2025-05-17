using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ROSE.Core.Configuration;
using ROSE.Core.Services;
using ROSE.Infrastructure.Data;
using ROSE.Infrastructure.Services;
using ROSE.UI.Services;
using ROSE.UI.ViewModels.Pages;
using ROSE.UI.ViewModels.Windows;
using ROSE.UI.Views.Pages;
using ROSE.UI.Views.Windows;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;

namespace ROSE.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c =>
            {
#pragma warning disable CS8604
                c.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory));
#pragma warning restore CS8604
            })
            .ConfigureServices((context, services) =>
            {
                // Core Services
                services.AddSingleton<ICryptoService, CryptoService>();
                services.AddSingleton<IDatabaseService, DatabaseService>();
                services.AddSingleton<IMachineKeyService, MachineKeyService>();

                // Database Services
                services.AddDbContextFactory<ProfileDbContext>((provider, options) =>
                {
                    AppPaths.EnsureDirectoriesExist();
                    options.UseSqlite($"Data Source={AppPaths.DatabasePath}");
                });
                services.AddScoped<IDatabaseService, DatabaseService>();

                // UI Services
                services.AddNavigationViewPageProvider();
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddHostedService<ApplicationHostService>();
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<INavigationService, NavigationService>();

                // Main Window with Navigation
                services.AddSingleton<INavigationWindow, MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                // Page View and View Models
                services.AddSingleton<DashboardPage>();
                services.AddSingleton<DashboardViewModel>();
                services.AddSingleton<DataPage>();
                services.AddSingleton<DataViewModel>();
                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<ProfileManagementPage>();
                services.AddSingleton<ProfileManagementViewModel>();
            }).Build();

        /// <summary>
        /// Gets services.
        /// </summary>
        public static IServiceProvider Services
        {
            get { return _host.Services; }
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            // Start the host first to ensure all services are initialized
            await _host.StartAsync();

            // Initialize database
            var databaseService = _host.Services.GetRequiredService<IDatabaseService>();
            databaseService.InitializeDatabase();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }
}
