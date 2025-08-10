using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Helpers;
using RepairTracking.Services;

namespace RepairTracking.ViewModels;

public partial class SendMailViewModel : ViewModelBase
{
    [ObservableProperty] private string _toEmail;
    [ObservableProperty] private string _subject;
    [ObservableProperty] private string _message;
    [ObservableProperty] private string _filePath;

    private readonly IDialogService _dialogService;

    public SendMailViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async Task SendEmail()
    {
        if (string.IsNullOrWhiteSpace(ToEmail) ||
            string.IsNullOrWhiteSpace(Message))
        {
            await _dialogService.OkMessageBox("Lütfen tüm alanları doldurun.", MessageTitleType.WarningTitle);
            return;
        }

        try
        {
            // SMTP istemcisi oluşturma
            var mailService = new NotificationFactory(new MailService(ToEmail,FilePath));
            mailService.SendMessage(Subject, Message);
            // var mailSerivce = new MailKitSmptClient();
            // mailSerivce.SendEmailAsync("",FilePath);
            await _dialogService.OkMessageBox("Mail başarıyla gönderildi.", MessageTitleType.SuccessTitle);

            ToEmail = string.Empty;
            Subject = string.Empty;
            Message = string.Empty;
        }
        catch (Exception ex)
        {
            await _dialogService.OkMessageBox($"E-posta gönderilemedi: {ex.Message}", MessageTitleType.SuccessTitle);
        }
    }

    public TopLevel View { get; set; }

    [RelayCommand]
    private async Task LoadFile()
    {
        if (View is not { } topLevel) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Dosya Seç",
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.All]
        });

        if (files.Count >= 1)
        {
            var file = files[0];
            await using var stream = await file.OpenReadAsync();
            FilePath = file.Name;
        }
    }
}