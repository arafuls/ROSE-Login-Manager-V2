using System.Collections.ObjectModel;

namespace ROSE.UI.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ProfileCardViewModel> _profiles = new();

        [RelayCommand]
        private void AddProfile()
        {
            var newProfile = new ProfileCardViewModel
            {
                AccountName = $"Profile {Profiles.Count + 1}",
                Email = $"profile{Profiles.Count + 1}.fillertext123@example.com"
            };
            Profiles.Add(newProfile);
        }

        [RelayCommand]
        private void LaunchAll()
        {
            // TODO: Implement launch all functionality
        }
    }
}
