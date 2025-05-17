using System.Windows.Media;

namespace ROSE.UI.ViewModels
{
    public partial class ProfileCardViewModel : ObservableObject
    {
        [ObservableProperty]
        private ImageSource? _profileImage;

        [ObservableProperty]
        private string _accountName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;
    }
}