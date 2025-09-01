using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Helpers;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;

namespace RepairTracking.ViewModels;

public partial class DeliveryDateViewModel(
    IRenovationRepository renovationRepository,
    IDialogService dialogService) : ViewModelBase
{
    [ObservableProperty] private DateTime? _deliveryDate;
    [ObservableProperty] private RenovationViewModel _renovationViewModel;

    public DateTimeOffset? DeliveryDateForPicker
    {
        get
        {
            if (DeliveryDate == null || DeliveryDate == default(DateTime))
                return null;
            return new DateTimeOffset(DeliveryDate.Value);
        }
        set
        {
            DeliveryDate = value?.DateTime;
            OnPropertyChanged(nameof(DeliveryDate));
        }
    }

    [RelayCommand]
    private async Task SaveDeliveryDate()
    {
        if (DeliveryDate == null)
        {
            await dialogService.OkMessageBox("Lütfen teslim tarihini seçiniz.", MessageTitleType.WarningTitle);
            return;
        }

        if (RenovationViewModel.RepairDate > DateOnly.FromDateTime((DateTime)DeliveryDate))
        {
            await dialogService.OkMessageBox("Teslimat tarihi, uygulama tarihinden eski olamalı!",
                MessageTitleType.WarningTitle);
            return;
        }

        var result = renovationRepository.UpdateRenovationDeliveryDate(RenovationViewModel.Id, DeliveryDate.Value);
        if (result)
        {
            RenovationViewModel.DeliveryDate = DeliveryDate.Value;
            renovationRepository.SaveChanges();
            await dialogService.OkMessageBox("Teslim tarihi başarıyla kaydedildi.", MessageTitleType.SuccessTitle);
        }
        else
            await dialogService.OkMessageBox(
                "Teslim tarihi kaydedilirken bir hata oluştu. Lütfen tekrar deneyiniz.",
                MessageTitleType.ErrorTitle);
    }
}