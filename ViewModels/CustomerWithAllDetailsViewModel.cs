using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuestPDF.Fluent;
using RepairTracking.Data.Models;
using RepairTracking.Helpers;
using RepairTracking.Reporting;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;
using RepairTracking.ViewModels.Factories;
using Serilog;

namespace RepairTracking.ViewModels;

public partial class CustomerWithAllDetailsViewModel : ViewModelBase
{
    [ObservableProperty] private string _name;

    [ObservableProperty] private string _surname;

    [ObservableProperty] private string _phoneNumber;

    [ObservableProperty] private int _id;

    [ObservableProperty] private string? _email;

    [ObservableProperty] private string? _address;

    [ObservableProperty] private bool _passive;
    [ObservableProperty] private int _customerId;

    [ObservableProperty] private CreatedUserViewModel _createdUser;

    [ObservableProperty] private ObservableCollection<CustomersVehicleViewModel> _customerVehicles;

    [ObservableProperty] private ObservableCollection<VehicleViewModel> _vehicles;

    [ObservableProperty] private ObservableCollection<RenovationViewModel>? _currentRenovations;

    private VehicleViewModel? _selectedVehicle;

    private List<Vehicle>? _recordedVehiclesByChassisNo;

    public UserProfileHeaderViewModel HeaderViewModel { get; private set; }

    private string _searchText;

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            OnPropertyChanged();
            if (!string.IsNullOrWhiteSpace(value))
            {
                var searchedRenovations = CurrentRenovations.Where(
                    x => x.Complaint.Contains(value) || x.Note.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                         x.Note.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                         x.RenovationDetails.Any(rd => rd.Name.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                                                       rd.Description.Contains(value,
                                                           StringComparison.OrdinalIgnoreCase))
                ).ToList();
                CurrentRenovations = new ObservableCollection<RenovationViewModel>(searchedRenovations);
            }
            else
            {
                if (SelectedVehicle != null)
                    CurrentRenovations = SelectedVehicle.Renovations;
            }
        }
    }

    public VehicleViewModel? SelectedVehicle
    {
        get => _selectedVehicle;
        set
        {
            SetProperty(ref _selectedVehicle, value);
            OnPropertyChanged();
            var renovations = value?.Renovations;
            CurrentRenovations = renovations;
        }
    }

    public TopLevel? View { get; set; }

    private readonly IDialogService _dialogService;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IRenovationRepository _renovationRepository;

    public CustomerWithAllDetailsViewModel(IUnitOfWork unitOfWork, IDialogService dialogService,
        IViewModelFactory viewModelFactory, int customerId)
    {
        _viewModelFactory = viewModelFactory;
        _dialogService = dialogService;
        _unitOfWork = unitOfWork;
        CustomerId = customerId;
        _customerRepository = unitOfWork.CustomersRepository;
        _vehicleRepository = unitOfWork.VehiclesRepository;
        _renovationRepository = unitOfWork.RenovationsRepository;

        GetCustomerDetails(customerId);
    }

    private void GetCustomerDetails(int customerId)
    {
        var customer = _customerRepository.GetCustomerWithAllDetails(customerId);

        VehicleViewModel? reloadSameSelectedVehicleAfterSavinRenovations = null;
        if (SelectedVehicle != null)
            reloadSameSelectedVehicleAfterSavinRenovations = SelectedVehicle;
        if (customer != null)
        {
            var vehicleByChassises = customer.Vehicles.Select(x => x.ChassisNo);
            _recordedVehiclesByChassisNo = _vehicleRepository.GetAllVehicleByChassises(vehicleByChassises);
            HeaderViewModel = new UserProfileHeaderViewModel(_dialogService, _viewModelFactory)
            {
                Email = customer.Email??"",
            };
            
            Name = customer.Name;
            Surname = customer.Surname;
            PhoneNumber = customer.PhoneNumber;
            Email = customer.Email;
            Address = customer.Address;
            Id = customer.Id;
            Passive = customer.Passive;
            CreatedUser = new CreatedUserViewModel
            {
                UserId = customer.CreatedUserNavigation.UserId,
                Id = customer.CreatedUserNavigation.Id,
                Name = customer.CreatedUserNavigation.Name,
                Surname = customer.CreatedUserNavigation.Surname
            };
            CustomerVehicles = new ObservableCollection<CustomersVehicleViewModel>(
                customer.CustomersVehicles.Select(cv => new CustomersVehicleViewModel
                {
                    CustomerId = cv.CustomerId,
                    VehicleId = cv.VehicleId,
                    CreatedDate = cv.CreatedDate,
                    UpdatedDate = cv.UpdatedDate,
                    Id = cv.Id,
                    Passive = cv.Passive,
                    Customer = this,
                    Vehicle = new VehicleViewModel
                    {
                        PlateNumber = cv.Vehicle.PlateNumber,
                        ChassisNo = cv.Vehicle.ChassisNo,
                        CustomerId = cv.Vehicle.CustomerId,
                        Model = cv.Vehicle.Model,
                        Color = cv.Vehicle.Color,
                        Type = cv.Vehicle.Type,
                        EngineNo = cv.Vehicle.EngineNo,
                        Km = cv.Vehicle.Km,
                        Fuel = cv.Vehicle.Fuel,
                        Id = cv.Vehicle.Id,
                        Passive = cv.Vehicle.Passive
                    }
                }));
            Vehicles = new ObservableCollection<VehicleViewModel>(
                customer.Vehicles.Where(v => !v.Passive).Select(v => new VehicleViewModel
                {
                    PlateNumber = v.PlateNumber,
                    ChassisNo = v.ChassisNo,
                    CustomerId = v.CustomerId,
                    Model = v.Model,
                    Color = v.Color,
                    Type = v.Type,
                    EngineNo = v.EngineNo,
                    Km = v.Km,
                    Fuel = v.Fuel,
                    Id = v.Id,
                    Passive = v.Passive,
                    Image = v.Image,
                    Renovations = new ObservableCollection<RenovationViewModel>(
                        v.Renovations.Select(r => new RenovationViewModel
                        {
                            Id = r.Id,
                            RepairDate = r.RepairDate,
                            DeliveryDate = r.DeliveryDate,
                            Complaint = r.Complaint ?? string.Empty,
                            Note = r.Note ?? string.Empty,
                            VehicleId = r.VehicleId,
                            CustomerName = Name,
                            CustomerSurname = Surname,
                            PhoneNumber = PhoneNumber,
                            Email = Email,
                            Address = Address,
                            Vehicle = new VehicleViewModel
                            {
                                PlateNumber = r.Vehicle.PlateNumber,
                                ChassisNo = r.Vehicle.ChassisNo,
                                CustomerId = r.Vehicle.CustomerId,
                                Model = r.Vehicle.Model,
                                Color = r.Vehicle.Color,
                                Type = r.Vehicle.Type,
                                EngineNo = r.Vehicle.EngineNo,
                                Km = r.Vehicle.Km,
                                Fuel = r.Vehicle.Fuel,
                                Id = r.Vehicle.Id,
                                Passive = r.Vehicle.Passive
                            },
                            RenovationDetails = new ObservableCollection<RenovationDetailViewModel>(
                                r.RenovationDetails.Select(rd => new RenovationDetailViewModel
                                {
                                    Description = rd.Description,
                                    Name = rd.Name,
                                    Price = rd.Price,
                                    TCode = rd.TCode,
                                    Note = rd.Note,
                                    Id = rd.Id,
                                    RenovationId = rd.RenovationId
                                }))
                        })),
                    HasPastRocerd = _recordedVehiclesByChassisNo != null &&
                                    _recordedVehiclesByChassisNo.Any(x => x.ChassisNo == v.ChassisNo && x.Passive),
                }));
        }

        if (reloadSameSelectedVehicleAfterSavinRenovations != null)
        {
            var vehicleViewModel = Vehicles.First(r => r.Id == reloadSameSelectedVehicleAfterSavinRenovations.Id);
            CurrentRenovations = vehicleViewModel.Renovations;
            SelectedVehicle = vehicleViewModel;
        }
    }

    // ... your other ViewModel properties and commands ...

    [RelayCommand]
    private async Task PrintRepairReport(RenovationViewModel renovationViewModel)
    {
        var updatedDate=renovationViewModel.UpdatedDate != null 
            ? renovationViewModel.UpdatedDate.Value.ToString("yyyyMMddHHmmss") 
            : DateTime.Now.ToString("yyyyMMddHHmmss");
        var file = await _dialogService.SaveFilePickerAsync(View, "Araç Kabul Raporu",
            $"{renovationViewModel.CustomerName}-{renovationViewModel.Complaint}-{updatedDate}"
        );
        string absolutePath = string.Empty;
        if (file is not null)
        {
            absolutePath = Uri.UnescapeDataString(file.Path.AbsolutePath);
            var report = new RepairReportDocument(renovationViewModel);
            report.GeneratePdf(absolutePath);
        }

        if (!string.IsNullOrWhiteSpace(absolutePath))
        {
            _renovationRepository.UpdateRenovationReportPath(renovationViewModel.Id, absolutePath);

            var pdfViewModel =
                _viewModelFactory.CreatePdfViewerViewModel(absolutePath);
            await _dialogService.OpenPdfViewerWindow(pdfViewModel);

            if (!string.IsNullOrWhiteSpace(renovationViewModel.Email))
            {
                var sendMailToUser = await _dialogService.YesNoMessageBox(
                    $"{renovationViewModel.CustomerName} {renovationViewModel.CustomerSurname} müşterisine raporu mail ile göndermek ister misiniz?",
                    "Mail Gönder");

                if (sendMailToUser)
                {
                    var mailService =
                        new NotificationFactory(new MailService(renovationViewModel.Email, absolutePath));
                    var mail = await _unitOfWork.MailRepository.GetMailTemplateAsync("TESLIMAT");
                    if (mail is not null)
                        mailService.SendMessage(mail.Subject, mail.Template,
                            $"{renovationViewModel.CustomerName} {renovationViewModel.CustomerSurname}");
                    else
                        await _dialogService.OkMessageBox("Teslimat şablonu bulunamadı.", MessageTitleType.ErrorTitle);
                }
            }
        }
    }

    [RelayCommand]
    private async Task ShowPastRecords(VehicleViewModel? vehicle)
    {
        if (vehicle != null)
        {
            if (vehicle.HasPastRocerd && !vehicle.IsShowingPastRecords && vehicle.ChassisNo != null)
            {
                var chasissNo = vehicle.ChassisNo;
                var vehicleIds = await _vehicleRepository.GetPassiveVehicleIdsByChassisNo(chasissNo);
                var pastRecords = _renovationRepository.GetRenovationsByVehcileIds(vehicleIds.ToArray());
                CurrentRenovations = new ObservableCollection<RenovationViewModel>(pastRecords.Select(x =>
                    new RenovationViewModel
                    {
                        Id = x.Id,
                        RepairDate = x.RepairDate,
                        DeliveryDate = x.DeliveryDate,
                        Complaint = x.Complaint ?? string.Empty,
                        Note = x.Note ?? string.Empty,
                        VehicleId = x.VehicleId,
                        CustomerName = x.Vehicle.Customer.Name,
                        CustomerSurname = x.Vehicle.Customer.Surname,
                        Passive = x.Passive,
                        Vehicle = new VehicleViewModel
                        {
                            PlateNumber = x.Vehicle.PlateNumber,
                            ChassisNo = x.Vehicle.ChassisNo,
                            CustomerId = x.Vehicle.CustomerId,
                            Model = x.Vehicle.Model,
                            Color = x.Vehicle.Color,
                            Type = x.Vehicle.Type,
                            EngineNo = x.Vehicle.EngineNo,
                            Km = x.Vehicle.Km,
                            Fuel = x.Vehicle.Fuel,
                            Id = x.Vehicle.Id,
                            Passive = x.Vehicle.Passive
                        },
                        RenovationDetails = new ObservableCollection<RenovationDetailViewModel>(
                            x.RenovationDetails.Select(rd => new RenovationDetailViewModel
                            {
                                Description = rd.Description,
                                Name = rd.Name,
                                Price = rd.Price,
                                TCode = rd.TCode,
                                Note = rd.Note,
                                Id = rd.Id,
                                RenovationId = rd.RenovationId
                            }))
                    }));
                vehicle.IsShowingPastRecords = true;
            }
            else
            {
                SelectedVehicle = vehicle;
                vehicle.IsShowingPastRecords = false;
            }
        }
    }

    [RelayCommand]
    public async Task OpenEditCustomerDialog()
    {
        var store = _viewModelFactory.CreateEditCustomerViewModel(Name, Surname, PhoneNumber, Email, Address, Id);
        var result = await _dialogService.OpenEditCustomerDialogWindow(store);
        if (result != null)
        {
            Name = result.Customer.Name;
            Surname = result.Customer.Surname;
            PhoneNumber = result.Customer.PhoneNumber;
            Email = result.Customer.Email;
            Address = result.Customer.Address;
            var response = await _customerRepository.UpdateAsync(Id, result.Customer);
            await _unitOfWork.SaveChangesAsync();
            if (response)
                await _dialogService.OkMessageBox("Kullanıcı başarıyla güncellendi."
                    , MessageTitleType.SuccessTitle);
        }
    }

    [RelayCommand]
    private async Task OpenVehicleDetails(VehicleViewModel? selectedCustomerModel)
    {
        int? vehicleId = null;
        if (selectedCustomerModel != null)
            vehicleId = selectedCustomerModel.Id;

        var vehicleDetailsViewModel = _viewModelFactory.CreateVehicleDetailsViewModel(
            $"{Name} {Surname}", vehicleId, CustomerId);
        await _dialogService.OpenVehicleDetailsDialogWindow(vehicleDetailsViewModel);
        GetCustomerDetails(Id);
    }

    [RelayCommand]
    private async Task OpenRepairDetailWindow(RenovationViewModel? repairDetailsViewModel)
    {
        if (SelectedVehicle != null)
        {
            var vehicleDetailsViewModel =
                _viewModelFactory.CreateSaveRepairDetailViewModel(SelectedVehicle, repairDetailsViewModel);
            await _dialogService.OpenRepairDetailsDialogWindow(vehicleDetailsViewModel);
            GetCustomerDetails(Id);
        }
        else
        {
            await _dialogService.OkMessageBox("Lütfen önce araç seçiniz.", MessageTitleType.WarningTitle);
        }
    }

    [RelayCommand]
    public async Task DeleteVehicleCommand(VehicleViewModel selectedVehicleModel)
    {
        var deleteResult = await _dialogService.YesNoMessageBox(
            $"{selectedVehicleModel.PlateNumber} Plakalı, {Name} müşterisine ait aracı silmek istediğinize emin misiniz?",
            MessageTitleType.WarningTitle);

        if (deleteResult)
        {
            await _vehicleRepository.DeleteVehicle(selectedVehicleModel.Id);
            await _unitOfWork.SaveChangesAsync();
            GetCustomerDetails(Id);
            await _dialogService.OkMessageBox("Araç başarıyla silindi.", MessageTitleType.SuccessTitle);
        }
    }

    [RelayCommand]
    private async Task DeleteRenovation(RenovationViewModel selectedVehicleModel)
    {
        var response = await _dialogService.YesNoMessageBox(
            $"{selectedVehicleModel.Vehicle?.PlateNumber} Plakalı, {selectedVehicleModel.CustomerName} {selectedVehicleModel.CustomerSurname} müşterisine ait " +
            $"işlemi silmek istediğinize emin misiniz?", MessageTitleType.WarningTitle);
        if (!response)
            return;

        _renovationRepository.DeleteRenovation(selectedVehicleModel.Id);
        GetCustomerDetails(Id);
        await _dialogService.OkMessageBox("İşlem başarıyla silindi.", MessageTitleType.SuccessTitle);
    }

    [RelayCommand]
    private async Task MakeInProgress(RenovationViewModel renovationViewModel)
    {
        var result = _renovationRepository.DeleteRenovationDeliveryDate(renovationViewModel.Id);
        if (result)
            await _dialogService.OkMessageBox("İşlem başarıyla güncellendi.", MessageTitleType.SuccessTitle);
        else
            await _dialogService.OkMessageBox("İşlem güncellenirken bir hata oluştu.", MessageTitleType.ErrorTitle);
        renovationViewModel.DeliveryDate = null;
    }

    [RelayCommand]
    private async Task OpenDeliveriyDateWindow(RenovationViewModel renovationViewModel)
    {
        var viewModel = _viewModelFactory.CreateDeliveryDateViewModel(renovationViewModel);
        await _dialogService.OpenDeliveryDateDialogWindow(viewModel);
    }
}

public partial class CustomersVehicleViewModel : ViewModelBase
{
    [ObservableProperty] private int _customerId;

    [ObservableProperty] private int _vehicleId;

    [ObservableProperty] private DateTime _createdDate;

    [ObservableProperty] private DateTime? _updatedDate;

    [ObservableProperty] private int _id;

    [ObservableProperty] private bool _passive;

    [ObservableProperty] private CustomerWithAllDetailsViewModel? _customer;

    [ObservableProperty] private VehicleViewModel? _vehicle;
}

public partial class CreatedUserViewModel : ViewModelBase
{
    [ObservableProperty] private Guid _userId;

    [ObservableProperty] private int _id;

    [ObservableProperty] private string? _name;

    [ObservableProperty] private string? _surname;
}

public partial class VehicleViewModel : ViewModelBase
{
    [ObservableProperty] private string? _plateNumber;

    [ObservableProperty] private string? _chassisNo;

    [ObservableProperty] private int _customerId;

    [ObservableProperty] private int? _model;

    [ObservableProperty] private string? _color;

    [ObservableProperty] private string? _type;

    [ObservableProperty] private string? _engineNo;

    [ObservableProperty] private int? _km;

    [ObservableProperty] private string? _fuel;

    [ObservableProperty] private int _id;

    [ObservableProperty] private bool _passive;

    [ObservableProperty] private byte[]? _image;

    [ObservableProperty] private ObservableCollection<RenovationViewModel>? _renovations;

    public bool HasPastRocerd { get; set; }

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ButtonText))]
    private bool _isShowingPastRecords;

    public string ButtonText => IsShowingPastRecords ? "Güncel Kayıtlar" : "Geçmiş Kayıtlar";
}

public partial class RenovationViewModel : ViewModelBase
{
    [ObservableProperty] private int _id;

    [ObservableProperty] private DateOnly _repairDate;

    [NotifyPropertyChangedFor(nameof(Status))] [ObservableProperty]
    private DateTime? _deliveryDate;

    [ObservableProperty] private string? _complaint;

    [ObservableProperty] private string? _note;
    [ObservableProperty] private int _vehicleId;

    [ObservableProperty] private VehicleViewModel? _vehicle;

    [ObservableProperty] private ObservableCollection<RenovationDetailViewModel>? _renovationDetails;
    [ObservableProperty] private string? _customerName;
    [ObservableProperty] private string? _customerSurname;
    [ObservableProperty] private string? _email;
    [ObservableProperty] private string? _phoneNumber;
    [ObservableProperty] private string? _address;
    [ObservableProperty] private bool? _passive;
    [ObservableProperty] private DateTime? _createdDate;
    [ObservableProperty] private DateTime? _updatedDate;

    public double TotalPrice => RenovationDetails?.Count > 0
        ? Math.Round(RenovationDetails.Sum(rd => rd.Price), 2)
        : 0.0;

    public string? Status => DeliveryDate == null
        ? "İşlemde"
        : "Teslim Edildi";
}

public partial class RenovationDetailViewModel : ViewModelBase
{
    [ObservableProperty] private string? _description;

    [ObservableProperty] private string? _name;

    [ObservableProperty] private double _price;

    [ObservableProperty] private string? _tCode;

    [ObservableProperty] private string? _note;

    [ObservableProperty] private int _id;

    [ObservableProperty] private int? _renovationId;
    public Guid TemporaryId { get; } = Guid.NewGuid();
}