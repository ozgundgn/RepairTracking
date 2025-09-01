using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Data.Models;
using RepairTracking.Helpers;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;

namespace RepairTracking.ViewModels;

public partial class AddOrUpdateUserViewModel : ViewModelBase
{
    [ObservableProperty] private string? _name;
    [ObservableProperty] private string? _surname;
    [ObservableProperty] private string? _username;

    [ObservableProperty] [Phone(ErrorMessage = "Telefon formatı geçersiz.")]
    private string? _phone;

    [ObservableProperty] private string? _password;

    [ObservableProperty] [EmailAddress(ErrorMessage = "E-posta formatı geçersiz. ")]
    private string? _email;

    private readonly IUserRepository _userRepository;
    private readonly IDialogService _dialogService;

    public AddOrUpdateUserViewModel(IUserRepository userRepository, IDialogService dialogService)
    {
        _userRepository = userRepository;
        _dialogService = dialogService;
    }

    [NotifyPropertyChangedFor(nameof(ShowPassword))] [ObservableProperty]
    private int? _userId;

    public bool ShowPassword => UserId == null;

    [ObservableProperty] private bool _isPasswordVisible;

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
    }

    [RelayCommand]
    private async Task SaveUser()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Surname) || string.IsNullOrWhiteSpace(Phone) ||
            string.IsNullOrWhiteSpace(Email) && UserId > 0 && string.IsNullOrWhiteSpace(Password))
        {
            await _dialogService.OkMessageBox("Lütfen tüm alanları doldurun.", MessageTitleType.WarningTitle);
            return;
        }
        
        ValidateAllProperties();
        if (HasErrors)
            return;

        var checkUser = await _userRepository.GetUserByEmailAndUsernameAndSurnameAsync(Phone, Name, Surname,UserId);
        if (checkUser != null)
        {
            await _dialogService.OkMessageBox("Bu kullanıcı adı veya email ile kayıt zaten mevcut.",
                MessageTitleType.WarningTitle);
            return;
        }

        if (UserId == null)
        {
            var createUserViewModel = new User
            {
                UserId = Guid.NewGuid(),
                Name = Name,
                Surname = Surname,
                UserName = Username,
                Phone = Phone,
                Password = Password,
                Email = Email
            };

            var result = await _userRepository.AddUserAsync(createUserViewModel);
            await _userRepository.SaveChangesAsync();
            if (result)
                await _dialogService.OkMessageBox("Kullanıcı başarıyla eklendi.", MessageTitleType.SuccessTitle);
            else
                await _dialogService.OkMessageBox("Kullanıcı eklenirken bir hata oluştu.", MessageTitleType.ErrorTitle);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Surname) ||
                string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Phone))
            {
                await _dialogService.OkMessageBox("Lütfen tüm alanları doldurun.", MessageTitleType.WarningTitle);
                return;
            }

            var result = await _userRepository.UpdateUserAsync((int)UserId, Name, Surname, Username, Phone, Email);
            if (result)
                await _dialogService.OkMessageBox("Kullanıcı başarıyla güncellendi.", MessageTitleType.SuccessTitle);
            else
                await _dialogService.OkMessageBox("Kullanıcı güncellenirken bir hata oluştu.",
                    MessageTitleType.ErrorTitle);
        }
    }
}