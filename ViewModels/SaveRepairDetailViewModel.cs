using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using RepairTracking.Data.Models;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.ViewModels;

public partial class SaveRepairDetailViewModel : ViewModelBase
{
    [ObservableProperty] private DateOnly _repairDate;
    [ObservableProperty] private DateTime? _deliveryDate;
    [ObservableProperty] private string? _complaint;
    [ObservableProperty] private string? _note;
    [ObservableProperty] private VehicleViewModel? _vehicle;
    [ObservableProperty] private RenovationDetailViewModel? _selectedRenovationDetail;

    private ObservableCollection<RenovationDetailViewModel> _renovationDetails;

    public ObservableCollection<RenovationDetailViewModel> RenovationDetails
    {
        get => _renovationDetails ??= new ObservableCollection<RenovationDetailViewModel>();
        set => SetProperty(ref _renovationDetails, value);
    }

    //to be added renovation details
    [ObservableProperty] private string? _detailDescription;

    [ObservableProperty] private string? _detailName;

    [ObservableProperty] private double _detailPrice;

    [ObservableProperty] private int? _detailTCode;

    [ObservableProperty] private string? _detailNote;

    private readonly IRenovationRepository _renovationRepository;
    public int? VehicleId { get; set; }
    public int Id { get; set; }
    private readonly int? _renovationId;
    public VehicleViewModel? SelectedVehicle { get; set; }
    public int? RenovationId => _renovationId;

    // THIS IS THE WRAPPER PROPERTY FOR THE VIEW TO BIND TO
    public DateTimeOffset? RepairDateForPicker
    {
        get
        {
            if (RepairDate == default)
                return null;

            return new DateTimeOffset(RepairDate.ToDateTime(new TimeOnly(0, 0)));
        }
        set
        {
            DateOnly newDate = DateOnly.FromDateTime(value?.DateTime ?? DateTime.Now);
            RepairDate = newDate;
            OnPropertyChanged(nameof(RepairDate));
        }
    }

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

    public SaveRepairDetailViewModel(IRenovationRepository renovationRepository, VehicleViewModel selectedVehicle,
        int? renovationId)
    {
        _renovationRepository = renovationRepository;
        _renovationId = renovationId;
        SelectedVehicle = selectedVehicle;
        GetRenovationDetails();
    }

    public void GetRenovationDetails()
    {
        if (_renovationId.HasValue)
        {
            var renovation = _renovationRepository.GetRenovationById(_renovationId.Value);
            if (renovation != null)
            {
                Id = renovation.Id;
                VehicleId = renovation.VehicleId;
                Vehicle = new VehicleViewModel()
                {
                    Id = (int)renovation.VehicleId,
                    PlateNumber = renovation.Vehicle?.PlateNumber,
                    Type = renovation.Vehicle?.Type,
                    Model = renovation.Vehicle?.Model,
                    Color = renovation.Vehicle?.Color
                };
                Complaint = renovation.Complaint;
                RepairDate = renovation.RepairDate;
                DeliveryDate = renovation.DeliveryDate;
                Note = renovation.Note;
                RenovationDetails = new ObservableCollection<RenovationDetailViewModel>(
                    renovation.RenovationDetails.Select(x => new RenovationDetailViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price,
                        Description = x.Description,
                        Note = x.Note,
                        TCode = x.TCode,
                        RenovationId = x.RenovationId
                    }));
            }
        }
    }

    [RelayCommand]
    void ClearInputBoxes()
    {
        SelectedRenovationDetail = null;
        DetailName = string.Empty;
        DetailPrice = 0;
        DetailDescription = string.Empty;
        DetailNote = string.Empty;
        DetailTCode = null;
    }

    [RelayCommand]
    private async void AddDetail()
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(DetailName) || DetailPrice <= 0)
        {
            var detailErrorMessage = MessageBoxManager
                .GetMessageBoxStandard("", "Lütfen geçerli bir isim ve fiyat girin.",
                    ButtonEnum.Ok);
            await detailErrorMessage.ShowAsync();
            return;
        }

        var newDetail = new RenovationDetailViewModel()
        {
            Name = DetailName,
            Price = DetailPrice,
            Description = DetailDescription,
            Note = DetailNote,
            TCode = DetailTCode,
            RenovationId = RenovationId,
        };

        RenovationDetails.Add(newDetail);

        // Clear the input boxes for the next entry
        ClearInputBoxes();
    }

    [RelayCommand(CanExecute = nameof(CanRemoveAndUpdateDetail))]
    private void RemoveDetail()
    {
        if (SelectedRenovationDetail != null)
        {
            RenovationDetails.Remove(SelectedRenovationDetail);
        }

        ClearInputBoxes();
    }

    [RelayCommand(CanExecute = nameof(CanRemoveAndUpdateDetail))]
    private void UpdateDetail()
    {
        if (SelectedRenovationDetail != null)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(DetailName) || DetailPrice <= 0)
            {
                var detailErrorMessage = MessageBoxManager
                    .GetMessageBoxStandard("", "Lütfen geçerli bir isim ve fiyat girin.",
                        ButtonEnum.Ok);
                detailErrorMessage.ShowAsync().GetAwaiter();
                return;
            }

            var repairDetail =
                RenovationDetails.FirstOrDefault(x => x.TemporaryId == SelectedRenovationDetail.TemporaryId);
            if (repairDetail != null)
            {
                repairDetail.Name = DetailName;
                repairDetail.Price = DetailPrice;
                repairDetail.Description = DetailDescription;
                repairDetail.Note = DetailNote;
                repairDetail.TCode = DetailTCode;
            }
        }

        ClearInputBoxes();
    }

    // This enables/disables the Remove button based on whether an item is selected
    private bool CanRemoveAndUpdateDetail() => SelectedRenovationDetail != null;

    // We need to manually tell the command to re-evaluate its CanExecute status
    // when the SelectedDetail property changes.
    partial void OnSelectedRenovationDetailChanged(RenovationDetailViewModel? value)
    {
        DetailName = value?.Name;
        DetailPrice = value?.Price ?? 0;
        DetailDescription = value?.Description;
        DetailNote = value?.Note;
        DetailTCode = value?.TCode;
        RemoveDetailCommand.NotifyCanExecuteChanged();
        UpdateDetailCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private async void SaveRepairAsync()
    {
        // You can access all the data here
        if (SelectedVehicle != null)
        {
            if (!RenovationId.HasValue)
            {
                var finalRepair = new Renovation()
                {
                    Complaint = Complaint,
                    RepairDate = RepairDate,
                    DeliveryDate = DeliveryDate,
                    Note = Note,
                    VehicleId = SelectedVehicle.Id,
                    RenovationDetails = RenovationDetails.Select(x => new RenovationDetail()
                    {
                        Description = x.Description,
                        Name = x.Name,
                        Price = x.Price,
                        TCode = x.TCode,
                        Note = x.Note
                    }).ToList()
                };
                _renovationRepository.AddRenovation(finalRepair);
            }
            else
            {
                // If we are updating an existing renovation, we need to ensure we set the ID
                var existingRenovation = _renovationRepository.GetRenovationById(RenovationId.Value);
                if (existingRenovation == null)
                {
                    var messageBoxControl = MessageBoxManager
                        .GetMessageBoxStandard("Hata", "Güncellenecek tamir kaydı bulunamadı.",
                            ButtonEnum.Ok);
                    await messageBoxControl.ShowAsync();
                    return;
                }

                var result =
                    _renovationRepository.DeleteRenovationDetails(existingRenovation.RenovationDetails.ToList());

                existingRenovation.Complaint = Complaint;
                existingRenovation.RepairDate = RepairDate;
                existingRenovation.DeliveryDate = DeliveryDate;
                existingRenovation.Note = Note;
                existingRenovation.VehicleId = SelectedVehicle.Id;
                existingRenovation.RenovationDetails = RenovationDetails.Select(x => new RenovationDetail()
                {
                    Description = x.Description,
                    Name = x.Name,
                    Price = x.Price,
                    TCode = x.TCode,
                    Note = x.Note
                }).ToList();

                _renovationRepository.UpdateRenovation(existingRenovation);
            }

            await _renovationRepository.SaveChangesAsync();
        }

        await _renovationRepository.SaveChangesAsync();
        var messageBox = MessageBoxManager
            .GetMessageBoxStandard("İşlem Başarılı", "Tamir bilgileri kaydedildi.",
                ButtonEnum.Ok);
        await messageBox.ShowAsync();
        // For now, let's just imagine it's saved.
        // In a real app, you would pass this 'finalRepair' object back
        // to the main window or a service layer.
    }
}