using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using QuestPDF.Fluent;
using ReactiveUI;
using RepairTracking.Data.Models;
using RepairTracking.Models;
using RepairTracking.Reporting;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.ViewModels;

public partial class CustomerWithAllDetailsViewModel : ViewModelBase
{
    public CustomerWithAllDetailsViewModel(ICustomersVehiclesRepository customersVehiclesRepository,
        IRenovationRepository renovationRepository)
    {
        _customersVehiclesRepository = customersVehiclesRepository;
        _renovationRepository = renovationRepository;
    }

    [ObservableProperty] private string _name;

    [ObservableProperty] private string _surname;

    [ObservableProperty] private string _phoneNumber;

    [ObservableProperty] private int _id;

    [ObservableProperty] private string? _email;

    [ObservableProperty] private string? _address;

    [ObservableProperty] private bool _passive;

    private ObservableCollection<CustomersVehicleViewModel> _customerVehicles;

    [ObservableProperty] private CreatedUserViewModel _createdUser;
    private string _searchText;
    private ObservableCollection<VehicleViewModel> _vehicles;

    private ObservableCollection<RenovationViewModel> _currentRenovations;
    private VehicleViewModel? _selectedVehicle;

    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICustomersVehiclesRepository _customersVehiclesRepository;
    private readonly IRenovationRepository _renovationRepository;
    private List<Vehicle>? _recordedVehiclesByChassisNo;

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

    public ObservableCollection<CustomersVehicleViewModel> CustomerVehicles
    {
        get => _customerVehicles;
        set
        {
            SetProperty(ref _customerVehicles, value);
            OnPropertyChanged();
        }
    }

    public ObservableCollection<RenovationViewModel> CurrentRenovations
    {
        get => _currentRenovations;
        set
        {
            SetProperty(ref _currentRenovations, value);
            OnPropertyChanged();
        }
    }

    public ObservableCollection<VehicleViewModel> Vehicles
    {
        get => _vehicles;
        set
        {
            SetProperty(ref _vehicles, value);
            OnPropertyChanged();
        }
    }

    public Interaction<EditCustomerViewModel, CustomerViewModel?> OpenEditCustomerDialogWindow { get; }
    public Interaction<VehicleDetailsViewModel, Unit> OpenVehicleDetailsDialogWindow { get; }
    public Interaction<SaveRepairDetailViewModel, Unit> OpenRepairDetailsDialogWindow { get; }

    public CustomerWithAllDetailsViewModel(ICustomerRepository repository, IVehicleRepository vehicleRepository,
        ICustomersVehiclesRepository customersVehiclesRepository, IRenovationRepository renovationRepository,
        int customerId)
    {
        _customerRepository = repository;
        _vehicleRepository = vehicleRepository;
        _customersVehiclesRepository = customersVehiclesRepository;
        _renovationRepository = renovationRepository;

        GetCustomerDetails(customerId);

        OpenEditCustomerDialogWindow = new Interaction<EditCustomerViewModel, CustomerViewModel?>();
        OpenVehicleDetailsDialogWindow = new Interaction<VehicleDetailsViewModel, Unit>();
        OpenRepairDetailsDialogWindow = new Interaction<SaveRepairDetailViewModel, Unit>();
    }

    public TopLevel? View { get; set; }

    // ... your other ViewModel properties and commands ...

    [RelayCommand]
    private async Task PrintRepairReport(RenovationViewModel renovationViewModel)
    {
        // 2. Open the "Save File" dialog
        if (View?.StorageProvider is not { } storageProvider)
        {
            // This should not happen in a normal desktop app.
            return;
        }

        var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Araç Kabul Raporu",
            SuggestedFileName = $"Rapor-{renovationViewModel.CustomerName}-{System.DateTime.Now:yyyy-MM-dd}",
            DefaultExtension = "pdf",
            FileTypeChoices = new[] { new FilePickerFileType("PDF Document") { Patterns = new[] { "*.pdf" } } }
        });

        // 3. If the user selected a file, generate and save the PDF
        if (file is not null)
        {
            // Using the path from the saved file, generate the PDF
            var report = new RepairReportDocument(renovationViewModel);
            report.GeneratePdf(file.Path.AbsolutePath);
        }

        if (!renovationViewModel.DeliveryDate.HasValue)
            _renovationRepository.UpdateRenovationDeliveryDate(renovationViewModel.Id, DateTime.Now);

        await _renovationRepository.SaveChangesAsync();
    }

    private void GetCustomerDetails(int customerId)
    {
        var customer = _customerRepository.GetCustomerWithAllDetails(customerId);

        VehicleViewModel reloadSameSelectedVehicleAfterSavinRenovations = null;
        if (SelectedVehicle != null)
            reloadSameSelectedVehicleAfterSavinRenovations = SelectedVehicle;
        if (customer != null)
        {
            var vehicleByChassises = customer.Vehicles.Select(x => x.ChassisNo);
            _recordedVehiclesByChassisNo = _vehicleRepository.GetAllVehicleByChassises(vehicleByChassises);

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
                customer.Vehicles.Select(v => new VehicleViewModel
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
            CurrentRenovations = Vehicles.First(r => r.Id == reloadSameSelectedVehicleAfterSavinRenovations.Id)
                .Renovations;
    }

    [RelayCommand]
    private async Task ShowPastRecords(VehicleViewModel? vehicle)
    {
        if (vehicle != null)
        {
            if (vehicle.HasPastRocerd && !vehicle.IsShowingPastRecords)
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
        var store = new EditCustomerViewModel()
        {
            Name = Name,
            Surname = Surname,
            PhoneNumber = PhoneNumber,
            Email = Email,
            Address = Address,
            Id = Id
        };
        var result = await OpenEditCustomerDialogWindow.Handle(store);
        if (result != null)
        {
            Name = result.Customer.Name;
            Surname = result.Customer.Surname;
            PhoneNumber = result.Customer.PhoneNumber;
            Email = result.Customer.Email;
            Address = result.Customer.Address;
            var response = await _customerRepository.UpdateAsync(Id, result.Customer);
            await _customerRepository.SaveChangesAsync();
            if (response)
            {
                var addedBox = MessageBoxManager
                    .GetMessageBoxStandard("İşlem Başarılı", "Kullanıcı başarıyla güncellendi.",
                        ButtonEnum.Ok);
                await addedBox.ShowAsync();
            }
        }
    }

    [RelayCommand]
    public async Task OpenVehicleDetails(VehicleViewModel? selectedCustomerModel)
    {
        int? vehicleId = null;
        if (selectedCustomerModel != null)
            vehicleId = selectedCustomerModel.Id;

        var vehicleDetailsViewModel = new VehicleDetailsViewModel(_vehicleRepository,
            Name + " " + Surname, _customersVehiclesRepository, _renovationRepository, vehicleId)
        {
            CustomerId = Id
        };
        var resul = await OpenVehicleDetailsDialogWindow.Handle(vehicleDetailsViewModel);
        GetCustomerDetails(Id);
    }

    [RelayCommand]
    private async Task OpenRepairDetailWindow(RenovationViewModel? repairDetailsViewModel)
    {
        int? renovationId = repairDetailsViewModel?.Id;
        if (SelectedVehicle != null)
        {
            var vehicleDetailsViewModel =
                new SaveRepairDetailViewModel(_renovationRepository, selectedVehicle: SelectedVehicle,
                    renovationId);
            var result = await OpenRepairDetailsDialogWindow.Handle(vehicleDetailsViewModel);
        }

        GetCustomerDetails(Id);
    }

    [RelayCommand]
    public async Task DeleteVehicleCommand(VehicleViewModel selectedVehicleModel)
    {
        var deleteWarning = MessageBoxManager
            .GetMessageBoxStandard("",
                $"{selectedVehicleModel.PlateNumber} Plakalı, {Name} müşterisine ait aracı silmek istediğinize emin misiniz?",
                ButtonEnum.YesNo);
        var deleteResult = await deleteWarning.ShowAsync();

        if (deleteResult == ButtonResult.Yes)
        {
            var deleteResponse = _vehicleRepository.DeleteVehicle(selectedVehicleModel.Id);
            await _vehicleRepository.SaveChangesAsync();
            if (deleteResponse)
            {
                GetCustomerDetails(Id);
                var deletedBox = MessageBoxManager
                    .GetMessageBoxStandard("İşlem Başarılı", "Araç başarıyla silindi.",
                        ButtonEnum.Ok);
                await deletedBox.ShowAsync();
            }
            else
            {
                var errorBox = MessageBoxManager
                    .GetMessageBoxStandard("İşlem Başarısız", "Araç silinirken bir hata oluştu.",
                        ButtonEnum.Ok);
                await errorBox.ShowAsync();
            }
        }
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

    [ObservableProperty] private CustomerWithAllDetailsViewModel _customer;

    [ObservableProperty] private VehicleViewModel _vehicle;
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
    [ObservableProperty] private string _plateNumber;

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

    [ObservableProperty] private ObservableCollection<RenovationViewModel> _renovations;

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
    [ObservableProperty] private int? _vehicleId;

    [ObservableProperty] private VehicleViewModel? _vehicle;

    private ObservableCollection<RenovationDetailViewModel> _renovationDetails;
    [ObservableProperty] private string _customerName;
    [ObservableProperty] private string _customerSurname;
    [ObservableProperty] private string? _email;
    [ObservableProperty] private string? _phoneNumber;
    [ObservableProperty] private string? _address;

    public ObservableCollection<RenovationDetailViewModel> RenovationDetails
    {
        get => _renovationDetails;
        set
        {
            SetProperty(ref _renovationDetails, value);
            OnPropertyChanged();
        }
    }

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

    [ObservableProperty] private int? _tCode;

    [ObservableProperty] private string? _note;

    [ObservableProperty] private int _id;

    [ObservableProperty] private int? _renovationId;

    public Guid TemporaryId { get; set; } = Guid.NewGuid();
}