using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Helpers;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;

namespace RepairTracking.ViewModels;

public partial class SendMailViewModel(
    IDialogService dialogService,
    IMailRepository mailRepository,
    IUserRepository userRepository)
    : ViewModelBase
{
    [ObservableProperty] private string _toEmail;
    [ObservableProperty] private string _subject;
    [ObservableProperty] private string _message;
    [ObservableProperty] private string _filePath;
    [ObservableProperty] private bool _clearableFile;
    [ObservableProperty] private int? _id;

    public TopLevel View { get; set; }

    [RelayCommand]
    private async Task DeliveryMessageTemplate()
    {
        var mail = await mailRepository.GetMailTemplateAsync("TESLIMAT");
        if (mail != null)
        {
            Subject = mail.Subject;
            Message = mail.Template;
            Id = mail.Id;
        }
        else
        {
            await dialogService.OkMessageBox("Teslimat şablonu bulunamadı.", MessageTitleType.ErrorTitle);
        }
    }

    [RelayCommand]
    private void EmptyMessageTemplate()
    {
        Subject = string.Empty;
        Message = string.Empty;
        Id = null;
    }

    [RelayCommand]
    private async Task SendEmail()
    {
        if (string.IsNullOrWhiteSpace(ToEmail) || string.IsNullOrWhiteSpace(Message))
        {
            await dialogService.OkMessageBox("Lütfen alıcı ve mesaj alanları doldurun.", MessageTitleType.WarningTitle);
            return;
        }

        var user = await userRepository.GetUserAsync(x => x.Email == ToEmail);
        try
        {
            // SMTP istemcisi oluşturma
            var mailService = new NotificationFactory(new MailService(ToEmail, FilePath));
            mailService.SendMessage(Subject, Message, user?.Name ?? "");
            await dialogService.OkMessageBox("Mail başarıyla gönderildi.", MessageTitleType.SuccessTitle);
            ToEmail = string.Empty;
            Subject = string.Empty;
            Message = string.Empty;
        }
        catch (Exception ex)
        {
            await dialogService.OkMessageBox($"E-posta gönderilemedi: {ex.Message}", MessageTitleType.ErrorTitle);
        }
    }


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
            FilePath = Uri.UnescapeDataString(file.Path.AbsolutePath);
            ClearableFile = true;
        }
    }

    [RelayCommand]
    private async Task SaveTemplate()
    {
        if (Id is not null)
        {
            if (string.IsNullOrWhiteSpace(Subject) || string.IsNullOrWhiteSpace(Message))
            {
                await dialogService.OkMessageBox("Konu ve mesaj alanları boş olamaz.", MessageTitleType.WarningTitle);
                return;
            }

            await mailRepository.SaveMailTemplateAsync((int)Id, Subject, Message);
            await dialogService.OkMessageBox("Şablon başarıyla kaydedildi.", MessageTitleType.SuccessTitle);
        }
    }

    [RelayCommand]
    private void ClearFile()
    {
        FilePath = string.Empty;
        ClearableFile = false;
    }
}