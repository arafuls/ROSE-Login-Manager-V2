using ROSE.UI.ViewModels.Pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ROSE.UI.Views.Pages
{
    public partial class ProfileManagementPage : Page
    {
        private readonly ProfileManagementViewModel _viewModel;

        public ProfileManagementPage(ProfileManagementViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the clicked element
            var clickedElement = e.OriginalSource as FrameworkElement;
            
            // If we clicked on a control or its container, don't deselect
            if (clickedElement is Control || 
                clickedElement is Panel || 
                clickedElement is ContentPresenter || 
                clickedElement is TextBlock ||
                clickedElement?.Parent is Control ||
                clickedElement?.Parent is Panel ||
                clickedElement?.Parent is ContentPresenter)
            {
                return;
            }

            // If we get here, we clicked on the background
            ProfilesDataGrid.SelectedItem = null;
        }
    }
}