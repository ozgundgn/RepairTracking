using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Models;

namespace RepairTracking.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    public string Username { get; set; }
    private string _password;

    public string Password
    {
        get => _password;
        set
        {
            SetProperty(ref _password, value);
            OnPropertyChanged();
        }
    }

    public IAsyncRelayCommand LoginCommand { get; }

    public LoginViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        LoginCommand = new AsyncRelayCommand(Login);
    }

    private async Task Login()
    {
        // Here you would typically validate the username and password
        // For now, we will just simulate a successful login
        if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
        {
            var user = new User() { Username = Username, UserId = "12345" }; // Simulated user ID
            AppState.Instance.SetUser(user);
            _mainWindowViewModel.NavigateToHome();
        }
    }
}