using Microsoft.Extensions.Hosting;
using ROSE.Core.Services;
using ROSE.UI.Views.Windows;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace ROSE.UI.Services
{
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISettingsService _settingsService;
        private INavigationWindow? _navigationWindow;

        public ApplicationHostService(
            IServiceProvider serviceProvider,
            ISettingsService settingsService)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await HandleActivationAsync();
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Creates main window during activation.
        /// </summary>
        private async Task HandleActivationAsync()
        {
            if (!Application.Current.Windows.OfType<MainWindow>().Any())
            {
                await InitializeMainWindowAsync();
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Initializes the main window and applies the theme from settings.
        /// </summary>
        private async Task InitializeMainWindowAsync()
        {
            try
            {
                _navigationWindow = _serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow;
                if (_navigationWindow == null)
                {
                    throw new InvalidOperationException("Failed to resolve INavigationWindow service");
                }

                ApplyThemeFromSettings();

                _navigationWindow.ShowWindow();
                _navigationWindow.Navigate(typeof(Views.Pages.DashboardPage));
            }
            catch (Exception ex)
            {
                // Log the error and potentially show a user-friendly message
                // TODO: Add proper logging
                throw new InvalidOperationException("Failed to initialize main window", ex);
            }
        }

        /// <summary>
        /// Applies the theme based on the current settings.
        /// </summary>
        private void ApplyThemeFromSettings()
        {
            var settings = _settingsService.GetSettings();
            var theme = settings.Theme.Equals("Light", StringComparison.OrdinalIgnoreCase)
                ? ApplicationTheme.Light
                : ApplicationTheme.Dark;

            ApplicationThemeManager.Apply(theme);
        }
    }
}