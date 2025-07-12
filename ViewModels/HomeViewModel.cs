using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using RepairTracking.Data.Models;
using RepairTracking.Models;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;

namespace RepairTracking.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    private ObservableCollection<VehicleCustomerModel> _allCustomersModels;
    private ObservableCollection<VehicleCustomerModel> _showedCustomersModels;
    [ObservableProperty] private VehicleCustomerModel _selectedCustomerModel;
    [ObservableProperty] private string _searchText;
    private readonly IVehicleRepository _repository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICustomersVehiclesRepository _customersVehiclesRepository;
    private readonly IRenovationRepository _renovationRepository;

    public ObservableCollection<VehicleCustomerModel> AllCustomersModels
    {
        get => _allCustomersModels;
        set
        {
            SetProperty(ref _allCustomersModels, value);
            OnPropertyChanged();
        }
    }

    public UserProfileHeaderViewModel HeaderViewModel { get; }
    public Interaction<AddCustomerViewModel, CustomerViewModel?> OpenAddCustomerDialogWindow { get; }
    public Interaction<VehicleDetailsViewModel, Unit> OpenVehicleDetailsDialogWindow { get; }
    public Interaction<CustomerWithAllDetailsViewModel, Unit> OpenCustomerDetailsDialogWindow { get; }

    public ObservableCollection<VehicleCustomerModel> ShowedCustomersModels
    {
        get => _showedCustomersModels;
        set
        {
            SetProperty(ref _showedCustomersModels, value);
            OnPropertyChanged();
        }
    }

    public string LoggedUser { get; set; }

    //Pagination
    private int _currentPage = 1;
    private int _pageSize = 15;

    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            SetProperty(ref _currentPage, value);
            _ = LoadPagedCustomers();
        }
    }

    public int TotalPages => (int)Math.Ceiling((double)_allCustomersModels.Count / _pageSize);

    public ReactiveCommand<Unit, Unit> NextPageCommand => ReactiveCommand.Create(() =>
    {
        if (CurrentPage < TotalPages)
            CurrentPage++;
    });


    public ReactiveCommand<Unit, Unit> PreviousPageCommand => ReactiveCommand.Create(() =>
    {
        if (CurrentPage > 1)
            CurrentPage--;
    });

    public HomeViewModel(IVehicleRepository repository, UserProfileHeaderViewModel headerViewModel,
        ICustomerRepository customerRepository, ICustomersVehiclesRepository customersVehiclesRepository,
        IVehicleRepository vehicleRepository, IRenovationRepository renovationRepository)
    {
        _repository = repository;
        HeaderViewModel = headerViewModel;
        _customerRepository = customerRepository;
        _customersVehiclesRepository = customersVehiclesRepository;
        _vehicleRepository = vehicleRepository;
        _renovationRepository = renovationRepository;
        _showedCustomersModels = new ObservableCollection<VehicleCustomerModel>();
        LoggedUser = AppServices.UserSessionService.CurrentUser?.Fullname ?? "Unknown User";
        OpenAddCustomerDialogWindow = new Interaction<AddCustomerViewModel, CustomerViewModel?>();
        OpenVehicleDetailsDialogWindow = new Interaction<VehicleDetailsViewModel, Unit>();
        OpenCustomerDetailsDialogWindow = new Interaction<CustomerWithAllDetailsViewModel, Unit>();
        LoadAllData();
    }

    [RelayCommand]
    private async Task OpenAddCustomerDialog()
    {
        var store = new AddCustomerViewModel()
        {
            ExistingCustomers = _allCustomersModels.ToList(),
        };
        var result = await OpenAddCustomerDialogWindow.Handle(store);
        if (result != null)
        {
            var entity = await _customerRepository.AddAsync(result.Customer);
            // Assuming you have a method to add the customer to the repository
            await _customerRepository.SaveChangesAsync();

            var customerId = entity.Id;
            var vehicleId = entity.Vehicles.First().Id;
            var CustomerVehicle = new CustomersVehicle()
            {
                CustomerId = customerId,
                VehicleId = vehicleId,
                CreatedDate = DateTime.Now
            };
            await _customersVehiclesRepository.Add(CustomerVehicle);
            await _customerRepository.SaveChangesAsync();
            await LoadPagedCustomers(); // Refresh the list after adding a new customer
        }
    }

    [RelayCommand]
    private async Task OpenVehicleDetails(VehicleCustomerModel selectedCustomerModel)
    {
        var vehicleId = selectedCustomerModel.VehicleId;
        var vehicleDetailsViewModel = new VehicleDetailsViewModel(_repository,
            selectedCustomerModel.Name + " " + selectedCustomerModel.Surname, _customersVehiclesRepository,
            _renovationRepository, vehicleId);
        var resul = await OpenVehicleDetailsDialogWindow.Handle(vehicleDetailsViewModel);
        LoadAllData(); // Refresh the list after viewing details
        await LoadPagedCustomers();
    }

    [RelayCommand]
    private async Task OpenCustomerDetails(VehicleCustomerModel selectedCustomerModel)
    {
        var customerId = selectedCustomerModel.CustomerId;
        var customerWithAllDetailsViewModel = new CustomerWithAllDetailsViewModel(_customerRepository,
            _vehicleRepository, _customersVehiclesRepository, _renovationRepository, customerId);
        var result = await OpenCustomerDetailsDialogWindow.Handle(customerWithAllDetailsViewModel);
        LoadAllData(); // Refresh the list after viewing details
        await LoadPagedCustomers();
    }

    private void LoadAllData()
    {
        var allData = _repository.GetVehicleCustomerModel();
        _allCustomersModels = new ObservableCollection<VehicleCustomerModel>(allData);
    }

    [RelayCommand]
    private async Task LoadPagedCustomers()
    {
        IEnumerable<VehicleCustomerModel> vehicles = new List<VehicleCustomerModel>();
        if (!string.IsNullOrWhiteSpace(SearchText) && _allCustomersModels.Any())
        {
            vehicles = _allCustomersModels
                .Where(c => c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                            c.Surname.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                            c.PlateNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            vehicles = _repository.GetVehicleCustomerModel();
            _allCustomersModels = new ObservableCollection<VehicleCustomerModel>(vehicles);
        }

        var vehicleCustomerModels = vehicles.ToArray();
        var result = vehicleCustomerModels.ToList().Skip((CurrentPage - 1) * _pageSize)
            .Take(_pageSize)
            .ToList();
        ShowedCustomersModels = new ObservableCollection<VehicleCustomerModel>(result);
    }

    public async Task UpdatePlateNumber(int vehicleId, string plateNumber)
    {
        await _repository.UpdatePlateNumber(vehicleId, plateNumber);
    }

    [RelayCommand]
    private async Task DeleteVehicle(VehicleCustomerModel vehicle)
    {
        var deleteWarning = MessageBoxManager
            .GetMessageBoxStandard("",
                $"{vehicle.PlateNumber} Plakalı, {vehicle.Name + " " + vehicle.Surname} müşterisine ait aracı silmek istediğinize emin misiniz?",
                ButtonEnum.YesNo);
        var deleteResult = await deleteWarning.ShowAsync();
        if (deleteResult == ButtonResult.Yes)
        {
            _repository.DeleteVehicle(vehicle.VehicleId);
            await _vehicleRepository.SaveChangesAsync();
            await LoadPagedCustomers(); // Refresh the list after deletion
        }
    }

    [RelayCommand]
    private async Task DeleteCustomer(VehicleCustomerModel vehicle)
    {
        var deleteWarning = MessageBoxManager
            .GetMessageBoxStandard("",
                $"{vehicle.Name + " " + vehicle.Surname} müşterisini silmek istediğinize emin misiniz?",
                ButtonEnum.YesNo);
        var deleteResult = await deleteWarning.ShowAsync();
        if (deleteResult == ButtonResult.Yes)
            await _repository.DeleteCustomerAsync(vehicle.CustomerId);
        
        await LoadPagedCustomers(); // Refresh the list after deletion

    }

    public async Task SaveChanges()
    {
        // Assuming you have a method to save changes in your repository
        await _repository.SaveChangesAsync();
    }
}