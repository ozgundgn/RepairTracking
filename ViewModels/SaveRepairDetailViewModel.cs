using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Data.Models;
using RepairTracking.Helpers;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;

namespace RepairTracking.ViewModels;

public partial class SaveRepairDetailViewModel : ViewModelBase
{
    [ObservableProperty] private DateOnly _repairDate;
    [ObservableProperty] private DateTime? _deliveryDate;

    [ObservableProperty] [Required(ErrorMessage = "Şikayet gereklidir!")]
    private string? _complaint;

    [ObservableProperty] private string? _note;
    [ObservableProperty] private VehicleViewModel? _vehicle;
    [ObservableProperty] private RenovationDetailViewModel? _selectedRenovationDetail;
    [ObservableProperty] private Renovation? _renovation;

    private ObservableCollection<RenovationDetailViewModel?> _renovationDetails;

    public ObservableCollection<RenovationDetailViewModel?> RenovationDetails
    {
        get => _renovationDetails;
        set => SetProperty(ref _renovationDetails, value);
    }

    //to be added renovation details
    [ObservableProperty] private string? _detailDescription;

    [ObservableProperty] private string? _detailName;

    [ObservableProperty] private double _detailPrice;

    [ObservableProperty] private string? _detailTCode;

    [ObservableProperty] private string? _detailNote;

    private readonly IRenovationRepository _renovationRepository;
    public int? VehicleId { get; set; }
    public int Id { get; set; }
    [ObservableProperty] private int? _renovationId;
    [ObservableProperty] private RenovationViewModel? _renovationViewModel;
    public VehicleViewModel? SelectedVehicle { get; set; }


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
            if (value == null)
            {
                RepairDate = default;
                OnPropertyChanged(nameof(RepairDate));
                return;
            }

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

    private readonly IUnitOfWork _unitOfWork;
    private readonly IDialogService _dialogService;

    public SaveRepairDetailViewModel(IUnitOfWork unitOfWork, VehicleViewModel selectedVehicle,
        IDialogService dialogService,
        RenovationViewModel? renovationViewModel)
    {
        _unitOfWork = unitOfWork;
        _dialogService = dialogService;
        _renovationRepository = unitOfWork.RenovationsRepository;
        RenovationViewModel = renovationViewModel;
        SelectedVehicle = selectedVehicle;

        GetRenovationDetails();
    }

    private void GetRenovationDetails()
    {
        // Renovation = _renovationRepository.GetRenovationById(RenovationId.Value);k
        if (RenovationViewModel != null)
        {
            Id = RenovationViewModel.Id;
            VehicleId = RenovationViewModel.VehicleId;
            Vehicle = new VehicleViewModel
            {
                Id = RenovationViewModel.VehicleId,
                PlateNumber = RenovationViewModel.Vehicle?.PlateNumber,
                Type = RenovationViewModel.Vehicle?.Type,
                Model = RenovationViewModel.Vehicle?.Model,
                Color = RenovationViewModel.Vehicle?.Color
            };
            Complaint = RenovationViewModel.Complaint;
            RepairDate = RenovationViewModel.RepairDate;
            DeliveryDate = RenovationViewModel.DeliveryDate;
            Note = RenovationViewModel.Note;
            RenovationDetails = new ObservableCollection<RenovationDetailViewModel>(
                RenovationViewModel.RenovationDetails.Select(x => new RenovationDetailViewModel
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
    private async Task AddDetail()
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(DetailName) || DetailPrice <= 0)
        {
            await _dialogService.OkMessageBox("Lütfen geçerli bir isim ve fiyat girin.", MessageTitleType.WarningTitle);
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
        if (RenovationDetails == null || RenovationDetails.Count == 0)
            RenovationDetails = new ObservableCollection<RenovationDetailViewModel>();

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
                _dialogService.OkMessageBox("Lütfen geçerli bir isim ve fiyat girin", MessageTitleType.WarningTitle);
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
        ValidateAllProperties();
        if (HasErrors)
            return;
        if (RepairDate == default)
        {
            await _dialogService.OkMessageBox("Lütfen geçerli bir tamir tarihi girin!", MessageTitleType.WarningTitle);
            return;
        }

        if (DeliveryDate != null && RepairDate > DateOnly.FromDateTime((DateTime)DeliveryDate))
        {
            await _dialogService.OkMessageBox("Teslimat tarihi, uygulama tarihinden eski olamalı!",
                MessageTitleType.WarningTitle);
            return;
        }

        if (SelectedVehicle != null)
        {
            if (RenovationViewModel == null && Renovation == null)
            {
                Renovation = new Renovation
                {
                    Complaint = Complaint,
                    RepairDate = RepairDate,
                    DeliveryDate = DeliveryDate,
                    Note = Note,
                    VehicleId = SelectedVehicle.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    RenovationDetails = RenovationDetails?.Select(x => new RenovationDetail
                    {
                        Description = x.Description,
                        Name = x.Name,
                        Price = x.Price,
                        TCode = x.TCode,
                        Note = x.Note
                    })?.ToList()
                };

                _renovationRepository.AddRenovation(Renovation);
            }
            else
            {
                // If we are updating an existing renovation, we need to ensure we set the ID
                Renovation ??= _renovationRepository.GetRenovationById(RenovationViewModel.Id);
                if (Renovation == null)
                    throw new NullReferenceException("Renovation not found");
                if (Renovation.RenovationDetails != null && Renovation.RenovationDetails.Count > 0)
                    _renovationRepository.DeleteRenovationDetails(Renovation.RenovationDetails.ToList());

                Renovation.Complaint = Complaint;
                Renovation.RepairDate = RepairDate;
                Renovation.DeliveryDate = DeliveryDate;
                Renovation.Note = Note;
                Renovation.VehicleId = SelectedVehicle.Id;
                Renovation.UpdatedDate = DateTime.Now;
                Renovation.RenovationDetails = RenovationDetails.Select(x => new RenovationDetail()
                {
                    Description = x.Description,
                    Name = x.Name,
                    Price = x.Price,
                    TCode = x.TCode,
                    Note = x.Note
                }).ToList();

                _renovationRepository.UpdateRenovation(Renovation);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        await _dialogService.OkMessageBox("Tamir bilgileri kaydedildi.", MessageTitleType.SuccessTitle);
    }
}