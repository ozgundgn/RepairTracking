using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Models;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;

namespace RepairTracking.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IUserRepository _userRepository;

    [ObservableProperty] [Required(ErrorMessage = "Kullanıcı adı boş olamaz.")]
    private string? _username;

    [ObservableProperty] [Required(ErrorMessage = "Şifre boş olamaz.")]
    private string? _password;

    [ObservableProperty] private string? _errorMessage;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsErrorNotVisible))]
    private bool _isErrorVisible;

    public bool IsErrorNotVisible => !IsErrorVisible;
    public IAsyncRelayCommand LoginCommand { get; }

    public LoginViewModel(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        LoginCommand = new AsyncRelayCommand(Login);
    }
    partial void OnUsernameChanged(string? value) => IsErrorVisible = false;
    partial void OnPasswordChanged(string? value) => IsErrorVisible = false;
    private async Task Login()
    {
        IsErrorVisible = false;
        ValidateAllProperties();
        if (HasErrors) return;
        var user = await _userRepository.GetUserAsync(Username!, Password!);
        if (user == null)
        {
            IsErrorVisible = true;
            ErrorMessage = "Kullanıcı Bulunamadı!";
            return;
        }

        var userInfo = new UserInfo()
        {
            GuidId = user.UserId,
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname
        };
        
        AppServices.UserSessionService.Login(userInfo);
        AppServices.NavigationService.NavigateToHome();
    }
}