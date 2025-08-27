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

public partial class SendMailViewModel : ViewModelBase
{
    [ObservableProperty] private string _toEmail;
    [ObservableProperty] private string _subject;
    [ObservableProperty] private string _message;
    [ObservableProperty] private string _filePath;
    [ObservableProperty] private bool _clearableFile;
    [ObservableProperty] private string? _customerName;
    [ObservableProperty] private int? _id;

    private readonly IDialogService _dialogService;
    private readonly IMailRepository _mailRepository;

    public SendMailViewModel(IDialogService dialogService, IMailRepository mailRepository)
    {
        _dialogService = dialogService;
        _mailRepository = mailRepository;
    }

    [RelayCommand]
    private async Task DeliveryMessageTemplate()
    {
        var mail = await _mailRepository.GetMailTemplateAsync("TESLIMAT");
        if (mail != null)
        {
            Subject = mail.Subject;
            Message = mail.Template;
            Id = mail.Id;
        }
        else
        {
            await _dialogService.OkMessageBox("Teslimat şablonu bulunamadı.", MessageTitleType.ErrorTitle);
        }
    }

    [RelayCommand]
    private async Task EmptyMessageTemplate()
    {
        Subject = string.Empty;
        Message = string.Empty;
        Id = null;
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
            var mailService = new NotificationFactory(new MailService(ToEmail, FilePath));
            mailService.SendMessage(Subject, Message, CustomerName ?? "");
            // var mailSerivce = new MailKitSmptClient();
            // mailSerivce.SendEmailAsync("",FilePath);
            await _dialogService.OkMessageBox("Mail başarıyla gönderildi.", MessageTitleType.SuccessTitle);

            ToEmail = string.Empty;
            Subject = string.Empty;
            Message = string.Empty;
            
        }
        catch (Exception ex)
        {
            await _dialogService.OkMessageBox($"E-posta gönderilemedi: {ex.Message}", MessageTitleType.ErrorTitle);
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
            FilePath = file.Path.AbsolutePath;
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
                await _dialogService.OkMessageBox("Konu ve mesaj alanları boş olamaz.", MessageTitleType.WarningTitle);
                return;
            }

            await _mailRepository.SaveMailTemplateAsync((int)Id, Subject, Message);
            await _dialogService.OkMessageBox("Şablon başarıyla kaydedildi.", MessageTitleType.SuccessTitle);
        }
    }

    [RelayCommand]
    private void ClearFile()
    {
        FilePath = string.Empty;
        ClearableFile = false;
    }
}