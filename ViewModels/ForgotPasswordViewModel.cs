using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Helpers;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;

namespace RepairTracking.ViewModels;

public partial class ForgotPasswordViewModel
    : ViewModelBase
{
    [ObservableProperty] private int _userId;
    [ObservableProperty] private string _email;

    [ObservableProperty] [Required(ErrorMessage = "Kod alanını doldurunuz!")]
    private string _code;

    [ObservableProperty] string _sendedCode = string.Empty;


    private NotificationFactory _notificationFactory;

    private readonly IUserRepository _userRepository;
    private readonly IDialogService _dialogService;

    public ForgotPasswordViewModel(IUserRepository userRepository, IDialogService dialogService)
    {
        _userRepository = userRepository;
        _dialogService = dialogService;
        _notificationFactory = new NotificationFactory(new MailService(Email));
    }

    [RelayCommand]
    private async Task ConfirmCode()
    {
        var result = await _userRepository.ConfirmUserCodeAsync(UserId, Code);
        if (result)
        {
            var user = await _userRepository.GetUserByIdAsync(UserId);
            _notificationFactory.SendMessage("",
                $"Şifreniz: {user.Password}. Lütfen bu şifreyi kullanarak giriş yapınız.");

            await _dialogService.OkMessageBox("Kod onaylandı. Şifreniz mail adresinize gönderildi.",
                MessageTitleType.SuccessTitle);
            _dialogService.CloseCurrentWindow();
        }
        else
            await _dialogService.OkMessageBox("Kod onaylanamadı. Lütfen tekrar deneyin.", MessageTitleType.ErrorTitle);
    }

    [RelayCommand]
    private async Task ResendCode()
    {
        var user = await _userRepository.GetUserByIdAsync(UserId);
        if (user != null && user.Email != null)
        {
            _notificationFactory.SendMessage("Şifre Hatırlatma Onay Kodu",
                $"Kodunuz: {SendedCode}. Lütfen bu kodu doğrulama ekranına giriniz.");
        }
    }
}