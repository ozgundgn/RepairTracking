using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Storage;
using RepairTracking.Services;
using RepairTracking.ViewModels.Factories;

namespace RepairTracking.ViewModels;

public partial class UserProfileHeaderViewModel : ViewModelBase
{
    [ObservableProperty] private string _username;

    private readonly IDialogService _dialogService;
    private readonly IViewModelFactory _viewModelFactory;
    public UserProfileHeaderViewModel(IDialogService dialogService, IViewModelFactory viewModelFactory)
    {
        _dialogService = dialogService;
        _viewModelFactory = viewModelFactory;
        Username = AppServices.UserSessionService.CurrentUser?.Fullname ?? "Unknown User";
    }

    [RelayCommand]
    public void LogoutCommand()
    {
        AppServices.UserSessionService.Logout();
        AppServices.NavigationService.NavigateToLogin();
    }
    
    [RelayCommand]
    private async Task OpenUsersWindow()
    {
        var viewModel = _viewModelFactory.CreateUserViewModel();
        await _dialogService.OpenUsersWindow(viewModel);
    }
}