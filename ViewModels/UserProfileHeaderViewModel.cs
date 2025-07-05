using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Services;

namespace RepairTracking.ViewModels;

public partial class UserProfileHeaderViewModel : ViewModelBase
{
    [ObservableProperty] private string _username;

    public UserProfileHeaderViewModel()
    {
        Username = AppServices.UserSessionService.CurrentUser?.Fullname ?? "Unknown User";
    }

    [RelayCommand]
    public void LogoutCommand()
    {
        AppServices.UserSessionService.Logout();
        AppServices.NavigationService.NavigateToLogin();
    }
}