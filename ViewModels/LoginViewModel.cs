using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Helpers;
using RepairTracking.Models;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;
using RepairTracking.ViewModels.Factories;

namespace RepairTracking.ViewModels;

public partial class LoginViewModel(
    IUserRepository userRepository,
    IDialogService dialogService,
    IViewModelFactory viewModelFactory) : ViewModelBase
{
    [ObservableProperty] [Required(ErrorMessage = "Kullanıcı adı boş olamaz.")]
    private string? _username;

    [ObservableProperty] [Required(ErrorMessage = "Şifre boş olamaz.")]
    private string? _password;

    [ObservableProperty] private string? _errorMessage;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsErrorNotVisible))]
    private bool _isErrorVisible;

    public bool IsErrorNotVisible => !IsErrorVisible;
    partial void OnUsernameChanged(string? value) => IsErrorVisible = false;
    partial void OnPasswordChanged(string? value) => IsErrorVisible = false;

    [RelayCommand]
    private async Task Login()
    {
        IsErrorVisible = false;
        ValidateAllProperties();
        if (HasErrors) return;
        var user = await userRepository.GetUserAsync(Username!, Password!);
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

    [RelayCommand]
    private async Task OpenChangePasswordWindow()
    {
        var changePasswordViewModel = viewModelFactory.CreateChangePasswordViewModel(Username);
        await dialogService.OpenChangePasswordDialogWindow(changePasswordViewModel);
    }

    [RelayCommand]
    private async Task OpenForgotPasswordWindow()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            await dialogService.OkMessageBox("Kullanıcı adını boş bırakmayınız.", MessageTitleType.WarningTitle);
            return;
        }

        var user = await userRepository.GetUserByUsernameAsync(Username);
        if (user == null)
        {
            await dialogService.OkMessageBox("Aktif kullanıcı bulunamadı.", MessageTitleType.WarningTitle);
            return;
        }

        Random rnd = new Random();
        var code = rnd.Next(1000, 10000).ToString();
        user.Code = code;
        await userRepository.UpdateUserCodeAsync(user.Id, code);
        if (user.Email != null)
        {
            // var mailService = new MailKitSmptClient();
            // mailService.SendEmailAsync("fsfsf");
            var smtp = new NotificationFactory(new MailService(user.Email));
            smtp.SendMessage("Şifre Hatırlatma",code,user.Name + " " + user.Surname);

            var forgotPassword = viewModelFactory.CreateForgotPasswordViewModel(user.Id,code,user.Email!);
            await dialogService.OpenForgotPasswordDialogWindow(forgotPassword);
        }
        else
        {
            await dialogService.OkMessageBox("Kullanıcının e-posta adresi bulunamadı.", MessageTitleType.WarningTitle);
        }
    }
}