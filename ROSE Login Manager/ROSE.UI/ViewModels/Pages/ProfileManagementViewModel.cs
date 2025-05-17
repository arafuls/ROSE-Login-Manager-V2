using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ROSE.Core.Models;
using ROSE.Core.Services;

namespace ROSE.UI.ViewModels.Pages
{
    public partial class ProfileManagementViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        [ObservableProperty]
        private string _title = "Profile Management";

        [ObservableProperty]
        private ObservableCollection<Profile> _profiles = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UpdateProfileCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteProfileCommand))]
        private Profile? _selectedProfile;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddProfileCommand))]
        [NotifyCanExecuteChangedFor(nameof(UpdateProfileCommand))]
        private string _newProfileName = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddProfileCommand))]
        [NotifyCanExecuteChangedFor(nameof(UpdateProfileCommand))]
        private string _newEmail = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddProfileCommand))]
        [NotifyCanExecuteChangedFor(nameof(UpdateProfileCommand))]
        private string _newPassword = string.Empty;

        public ProfileManagementViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadProfilesAsync().ConfigureAwait(false);
        }

        private void ClearAllFields()
        {
            NewProfileName = string.Empty;
            NewEmail = string.Empty;
            NewPassword = string.Empty;
            SelectedProfile = null;
        }

        partial void OnSelectedProfileChanged(Profile? value)
        {
            if (value != null)
            {
                NewProfileName = value.ProfileName;
                NewEmail = value.Email;
                NewPassword = string.Empty;
            }
            else
            {
                ClearAllFields();
            }
        }

        [RelayCommand]
        private async Task LoadProfilesAsync()
        {
            try
            {
                var profiles = await _databaseService.GetAllProfilesAsync();
                Profiles = new ObservableCollection<Profile>(profiles);
            }
            catch (Exception ex)
            {
                // TODO: Show error message to user
                System.Diagnostics.Debug.WriteLine($"Error loading profiles: {ex.Message}");
            }
        }

        [RelayCommand(CanExecute = nameof(CanAddProfile))]
        private async Task AddProfileAsync()
        {
            try
            {
                var profile = new Profile
                {
                    ProfileName = NewProfileName,
                    Email = NewEmail,
                    Status = "Active"
                };

                await _databaseService.AddProfileAsync(profile, NewPassword);
                await LoadProfilesAsync();
                ClearAllFields();
            }
            catch (Exception ex)
            {
                // TODO: Show error message to user
                System.Diagnostics.Debug.WriteLine($"Error adding profile: {ex.Message}");
            }
        }

        [RelayCommand(CanExecute = nameof(CanUpdateProfile))]
        private async Task UpdateProfileAsync()
        {
            if (SelectedProfile == null) return;

            try
            {
                // Create a new profile with updated values
                var updatedProfile = new Profile
                {
                    ProfileName = NewProfileName,
                    Email = NewEmail,
                    Status = SelectedProfile.Status
                };

                // Update the profile in the database
                await _databaseService.UpdateProfileAsync(updatedProfile, NewPassword);
                await LoadProfilesAsync();
                ClearAllFields();
            }
            catch (Exception ex)
            {
                // TODO: Show error message to user
                System.Diagnostics.Debug.WriteLine($"Error updating profile: {ex.Message}");
            }
        }

        [RelayCommand(CanExecute = nameof(CanDeleteProfile))]
        private async Task DeleteProfileAsync()
        {
            if (SelectedProfile == null) return;

            try
            {
                await _databaseService.DeleteProfileAsync(SelectedProfile.ProfileName);
                await LoadProfilesAsync();
                ClearAllFields();
            }
            catch (Exception ex)
            {
                // TODO: Show error message to user
                System.Diagnostics.Debug.WriteLine($"Error deleting profile: {ex.Message}");
            }
        }

        private bool CanAddProfile()
        {
            // Cannot add if a profile is selected
            if (SelectedProfile != null) return false;

            // All fields must be filled
            if (string.IsNullOrWhiteSpace(NewProfileName) ||
                string.IsNullOrWhiteSpace(NewEmail) ||
                string.IsNullOrWhiteSpace(NewPassword))
            {
                return false;
            }

            // Email must be valid
            if (!EmailRegex.IsMatch(NewEmail))
            {
                return false;
            }

            // Profile name must not already exist
            if (Profiles.Any(p => p.ProfileName.Equals(NewProfileName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            // Email must not already exist
            if (Profiles.Any(p => p.Email.Equals(NewEmail, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            return true;
        }

        private bool CanUpdateProfile()
        {
            if (SelectedProfile == null) return false;

            // Email must be valid if changed
            if (NewEmail != SelectedProfile.Email && !EmailRegex.IsMatch(NewEmail))
            {
                return false;
            }

            // Check if any field has been changed
            bool hasChanges = 
                NewProfileName != SelectedProfile.ProfileName ||
                NewEmail != SelectedProfile.Email ||
                !string.IsNullOrWhiteSpace(NewPassword);

            // If changing profile name, check if it already exists
            if (NewProfileName != SelectedProfile.ProfileName &&
                Profiles.Any(p => p.ProfileName.Equals(NewProfileName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            // If changing email, check if it already exists
            if (NewEmail != SelectedProfile.Email &&
                Profiles.Any(p => p.Email.Equals(NewEmail, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            return hasChanges;
        }

        private bool CanDeleteProfile()
        {
            return SelectedProfile != null;
        }
    }
}