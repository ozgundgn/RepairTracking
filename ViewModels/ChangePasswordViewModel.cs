using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Helpers;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;

namespace RepairTracking.ViewModels;

public partial class ChangePasswordViewModel(IUserRepository userRepository, IDialogService dialogService)
    : ViewModelBase
{
    //  [Required(ErrorMessage = "Eski şifre boş olamaz.")]
    [ObservableProperty] private string _currentPassword = string.Empty;

    // [ObservableProperty] [Required(ErrorMessage = "Yeni şifre boş olamaz.")]
    [ObservableProperty] private string _newPassword = string.Empty;

    // [ObservableProperty] [Required(ErrorMessage = "Yeni şifre tekrar boş olamaz.")]
    [ObservableProperty] private string _confirmPassword = string.Empty;

    // [ObservableProperty] [Required(ErrorMessage = "Kullanıcı adı boş olamaz.")]
    [ObservableProperty] private string _username;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsErrorNotVisible))]
    private bool _isErrorVisible;

    public bool IsErrorNotVisible => !IsErrorVisible;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ConfirmPasswordEyeIconGeometry))]
    private bool _isConfirmPasswordVisible;

    [ObservableProperty] private bool _isCurrentPasswordVisible;
    [ObservableProperty] private bool _isNewPasswordVisible;

    [ObservableProperty] private string _errorMessage = string.Empty;

    private string _confirmPasswordEyeIconGeometry;
    private string _currentPasswordEyeIconGeometry;
    private string _newPasswordEyeIconGeometry;

    public string ConfirmPasswordEyeIconGeometry
    {
        get => _confirmPasswordEyeIconGeometry;
        set
        {
            SetProperty(ref _confirmPasswordEyeIconGeometry, value);
            EyeIcon(IsConfirmPasswordVisible);
        }
    }

    public string CurrentPasswordEyeIconGeometry
    {
        get => _currentPasswordEyeIconGeometry;
        set
        {
            SetProperty(ref _currentPasswordEyeIconGeometry, value);
            EyeIcon(IsCurrentPasswordVisible);
        }
    }

    public string NewPasswordEyeIconGeometry
    {
        get => _newPasswordEyeIconGeometry;
        set
        {
            SetProperty(ref _newPasswordEyeIconGeometry, value);
            EyeIcon(IsNewPasswordVisible);
        }
    }


    partial void OnUsernameChanged(string? value) => IsErrorVisible = false;
    partial void OnCurrentPasswordChanged(string? value) => IsErrorVisible = false;
    partial void OnNewPasswordChanged(string? value) => IsErrorVisible = false;
    partial void OnConfirmPasswordChanged(string? value) => IsErrorVisible = false;


    [RelayCommand]
    private async Task ChangePassword()
    {
        //check if current password is correct
        IsErrorVisible = false;
        // ValidateAllProperties();
        // if (HasErrors) return;

        if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(CurrentPassword) ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            IsErrorVisible = true;
            ErrorMessage = "Tüm alanlar doldurulmalıdır.";
            return;
        }

        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword) ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ErrorMessage = "Tüm alanlar doldurulmalıdır.";
            await dialogService.OkMessageBox(ErrorMessage, MessageTitleType.WarningTitle);
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "Yeni şifre ve onay şifresi eşleşmiyor.";
            await dialogService.OkMessageBox(ErrorMessage, MessageTitleType.WarningTitle);
            return;
        }

        var user = await userRepository.GetUserAsync(Username, CurrentPassword);

        if (user == null)
        {
            ErrorMessage = "Geçerli bir kullanıcı bulunamadı. Lütfen mevcut şifrenizi kontrol edin.";
            await dialogService.OkMessageBox(ErrorMessage, MessageTitleType.WarningTitle);
            return;
        }

        // Update the user's password
        user.Password = NewPassword;
        var result = await userRepository.UpdateUserPasswordAsync(user.Id, user.Password);
        if (result == true)
        {
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;

            ErrorMessage = "Şifre başarıyla değiştirildi.";
            await dialogService.OkMessageBox(ErrorMessage, MessageTitleType.SuccessTitle);
            return;
        }

        ErrorMessage = "Şifre değişikliği başarısız oldu. Lütfen tekrar deneyin.";
        await dialogService.OkMessageBox(ErrorMessage, MessageTitleType.ErrorTitle);
        return;

        await dialogService.OkMessageBox("Şifre değişikliği başarılı.", MessageTitleType.SuccessTitle);
    }

    [RelayCommand]
    private async Task ToggleConfirmPasswordVisibility()
    {
        IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
        ConfirmPasswordEyeIconGeometry = EyeIcon(IsConfirmPasswordVisible);
    }

    [RelayCommand]
    private async Task ToggleCurrentPasswordVisibility()
    {
        IsCurrentPasswordVisible = !IsCurrentPasswordVisible;
    }

    [RelayCommand]
    private async Task ToggleNewPasswordVisibility()
    {
        IsNewPasswordVisible = !IsNewPasswordVisible;
    }

    private string EyeIcon(bool isVisible)
    {
        return isVisible
            ? "M2.21967 2.21967C1.9534 2.48594 1.9292 2.9026 2.14705 3.19621L2.21967 3.28033L6.25424 7.3149C4.33225 8.66437 2.89577 10.6799 2.29888 13.0644C2.1983 13.4662 2.4425 13.8735 2.84431 13.9741C3.24613 14.0746 3.6534 13.8305 3.75399 13.4286C4.28346 11.3135 5.59112 9.53947 7.33416 8.39452L9.14379 10.2043C8.43628 10.9258 8 11.9143 8 13.0046C8 15.2138 9.79086 17.0046 12 17.0046C13.0904 17.0046 14.0788 16.5683 14.8004 15.8608L20.7197 21.7803C21.0126 22.0732 21.4874 22.0732 21.7803 21.7803C22.0466 21.5141 22.0708 21.0974 21.8529 20.8038L21.7803 20.7197L15.6668 14.6055L15.668 14.604L14.4679 13.4061L11.598 10.5368L11.6 10.536L8.71877 7.65782L8.72 7.656L7.58672 6.52549L3.28033 2.21967C2.98744 1.92678 2.51256 1.92678 2.21967 2.21967ZM10.2041 11.2655L13.7392 14.8006C13.2892 15.2364 12.6759 15.5046 12 15.5046C10.6193 15.5046 9.5 14.3853 9.5 13.0046C9.5 12.3287 9.76824 11.7154 10.2041 11.2655ZM12 5.5C10.9997 5.5 10.0291 5.64807 9.11109 5.925L10.3481 7.16119C10.8839 7.05532 11.4364 7 12 7C15.9231 7 19.3099 9.68026 20.2471 13.4332C20.3475 13.835 20.7546 14.0794 21.1565 13.9791C21.5584 13.8787 21.8028 13.4716 21.7024 13.0697C20.5994 8.65272 16.6155 5.5 12 5.5ZM12.1947 9.00928L15.996 12.81C15.8942 10.7531 14.2472 9.10764 12.1947 9.00928Z"
            : "M2.21967 2.21967C1.9534 2.48594 1.9292 2.9026 2.14705 3.19621L2.21967 3.28033L6.25424 7.3149C4.33225 8.66437 2.89577 10.6799 2.29888 13.0644C2.1983 13.4662 2.4425 13.8735 2.84431 13.9741C3.24613 14.0746 3.6534 13.8305 3.75399 13.4286C4.28346 11.3135 5.59112 9.53947 7.33416 8.39452L9.14379 10.2043C8.43628 10.9258 8 11.9143 8 13.0046C8 15.2138 9.79086 17.0046 12 17.0046C13.0904 17.0046 14.0788 16.5683 14.8004 15.8608L20.7197 21.7803C21.0126 22.0732 21.4874 22.0732 21.7803 21.7803C22.0466 21.5141 22.0708 21.0974 21.8529 20.8038L21.7803 20.7197L15.6668 14.6055L15.668 14.604L14.4679 13.4061L11.598 10.5368L11.6 10.536L8.71877 7.65782L8.72 7.656L7.58672 6.52549L3.28033 2.21967C2.98744 1.92678 2.51256 1.92678 2.21967 2.21967ZM10.2041 11.2655L13.7392 14.8006C13.2892 15.2364 12.6759 15.5046 12 15.5046C10.6193 15.5046 9.5 14.3853 9.5 13.0046C9.5 12.3287 9.76824 11.7154 10.2041 11.2655ZM12 5.5C10.9997 5.5 10.0291 5.64807 9.11109 5.925L10.3481 7.16119C10.8839 7.05532 11.4364 7 12 7C15.9231 7 19.3099 9.68026 20.2471 13.4332C20.3475 13.835 20.7546 14.0794 21.1565 13.9791C21.5584 13.8787 21.8028 13.4716 21.7024 13.0697C20.5994 8.65272 16.6155 5.5 12 5.5ZM12.1947 9.00928L15.996 12.81C15.8942 10.7531 14.2472 9.10764 12.1947 9.00928Z"; // 👁‍🗨️ icon
    }
}