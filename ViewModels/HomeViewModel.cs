using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuestPDF.Fluent;
using ReactiveUI;
using RepairTracking.Data.Models;
using RepairTracking.Helpers;
using RepairTracking.Models;
using RepairTracking.Reporting;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;
using RepairTracking.ViewModels.Factories;

namespace RepairTracking.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<VehicleCustomerModel> _allCustomersModels;
    private ObservableCollection<VehicleCustomerModel> _showedCustomersModels;
    [ObservableProperty] private string _previous="<< Önceki";
    [ObservableProperty] private VehicleCustomerModel _selectedCustomerModel;

    private bool _passiveVehiclesChecked;

    public bool PassiveVehiclesChecked
    {
        get => _passiveVehiclesChecked;
        set
        {
            SetProperty(ref _passiveVehiclesChecked, value);
            OnPropertyChanged();
            LoadPagedCustomers();
        }
    }

    private string _searchText;

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            OnPropertyChanged();
            LoadPagedCustomers();
        }
    }

    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICustomersVehiclesRepository _customersVehiclesRepository;

    public UserProfileHeaderViewModel HeaderViewModel => new(_dialogService, _viewModelFactory);

    // public Interaction<AddCustomerViewModel, CustomerViewModel?> OpenAddCustomerDialogWindow { get; }

    public ObservableCollection<VehicleCustomerModel> ShowedCustomersModels
    {
        get => _showedCustomersModels;
        set
        {
            SetProperty(ref _showedCustomersModels, value);
            OnPropertyChanged();
        }
    }

    //Pagination
    private int _currentPage = 1;
    private readonly int _pageSize = 15;

    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            SetProperty(ref _currentPage, value);
            _ = LoadPagedCustomers();
        }
    }

    public int TotalPages => (int)Math.Ceiling((double)AllCustomersModels.Count / _pageSize);

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

    private readonly IUnitOfWork _unitOfWork;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IDialogService _dialogService;
    public Func<TopLevel?> GetTopLevel { get; set; }

    public HomeViewModel(IUnitOfWork unitOfWork, IViewModelFactory viewModelFactory, IDialogService dialogService)
    {
        _unitOfWork = unitOfWork;
        _viewModelFactory = viewModelFactory;
        _dialogService = dialogService;

        _customerRepository = unitOfWork.CustomersRepository;
        _customersVehiclesRepository = unitOfWork.CustomersVehiclesRepository;
        _vehicleRepository = unitOfWork.VehiclesRepository;
        // OpenAddCustomerDialogWindow = new Interaction<AddCustomerViewModel, CustomerViewModel?>();

        Initialize();
    }

    private void Initialize()
    {
        LoadAllData();
        LoadPagedCustomers();
    }

    [RelayCommand]
    private async Task OpenAddCustomerDialog()
    {
        var store = _viewModelFactory.CreateAddCustomerViewModel(AllCustomersModels);
        // var result = await OpenAddCustomerDialogWindow.Handle(store);
        var result = await _dialogService.OpenAddCustomerDialogAsync(store);
        if (result != null)
        {
            var entity = await _customerRepository.AddAsync(result.Customer);
            await _unitOfWork.SaveChangesAsync();

            var customerId = entity.Id;
            var vehicleId = entity.Vehicles.First().Id;
            var customerVehicle = new CustomersVehicle()
            {
                CustomerId = customerId,
                VehicleId = vehicleId,
                CreatedDate = DateTime.Now
            };
            await _customersVehiclesRepository.Add(customerVehicle);
            await _unitOfWork.SaveChangesAsync();
            Initialize(); // Refresh the list after adding a new customer
        }
    }

    [RelayCommand]
    private async Task OpenVehicleDetails(VehicleCustomerModel selectedCustomerModel)
    {
        var vehicleId = selectedCustomerModel.VehicleId;
        var vehicleDetailsViewModel = _viewModelFactory.CreateVehicleDetailsViewModel(
            selectedCustomerModel.Name + " " + selectedCustomerModel.Surname, vehicleId,
            selectedCustomerModel.CustomerId);
        await _dialogService.OpenVehicleDetailsDialogWindow(vehicleDetailsViewModel);
        Initialize();
    }

    [RelayCommand]
    private async Task OpenCustomerDetails(VehicleCustomerModel selectedCustomerModel)
    {
        var customerWithAllDetailsViewModel =
            _viewModelFactory.CreateCustomerWithAllDetailsViewModel(selectedCustomerModel.CustomerId);
        await _dialogService.OpenCustomerDetailsDialogWindow(customerWithAllDetailsViewModel);
        Initialize();
    }

    private void LoadAllData()
    {
        var allData = _vehicleRepository.GetVehicleCustomerModel();
        AllCustomersModels = new ObservableCollection<VehicleCustomerModel>(allData);
    }

    [RelayCommand]
    private async Task GetActiveAndPassiveVehicles()
    {
    }

    [RelayCommand]
    private Task LoadPagedCustomers()
    {
        IEnumerable<VehicleCustomerModel> vehicles =
            PassiveVehiclesChecked ? AllCustomersModels : AllCustomersModels.Where(x => !x.Passive);

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            vehicles = vehicles
                .Where(c => c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                            c.Surname.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                            c.PlateNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        var vehicleCustomerModels = vehicles.ToArray();
        var result = vehicleCustomerModels.ToList().Skip((CurrentPage - 1) * _pageSize)
            .Take(_pageSize)
            .ToList();
        ShowedCustomersModels = new ObservableCollection<VehicleCustomerModel>(result);
        return Task.CompletedTask;
    }

    public async Task UpdatePlateNumber(int vehicleId, string plateNumber)
    {
        await _vehicleRepository.UpdatePlateNumber(vehicleId, plateNumber);
    }

    [RelayCommand]
    private async Task DeleteVehicle(VehicleCustomerModel vehicle)
    {
        var deleteResult = await _dialogService.YesNoMessageBox(
            $"{vehicle.PlateNumber} Plakalı, {vehicle.Name + " " + vehicle.Surname} müşterisine ait aracı silmek istediğinize emin misiniz?",
            MessageTitleType.WarningTitle);
        if (deleteResult)
        {
            await _vehicleRepository.DeleteVehicle(vehicle.VehicleId);
            await _unitOfWork.SaveChangesAsync();
            Initialize();
        }
    }

    [RelayCommand]
    private async Task DeleteCustomer(VehicleCustomerModel vehicle)
    {
        var deleteResult =
            await _dialogService.YesNoMessageBox(
                $"{vehicle.Name + " " + vehicle.Surname} müşterisini silmek istediğinize emin misiniz?",
                MessageTitleType.WarningTitle);
        if (deleteResult)
            await _customerRepository.DeleteCustomerAsync(vehicle.CustomerId);
        Initialize(); // Refresh the list after deletion
    }

    [RelayCommand]
    private async Task ShowLastRenovation(VehicleCustomerModel vehicle)
    {
        if (GetTopLevel() is not { } topLevel)
            return;

        var renovation = _unitOfWork.RenovationsRepository.GetLastRenovation(vehicle.VehicleId);
        if (renovation == null)
        {
            await _dialogService.OkMessageBox("Bu araç için henüz bir tamir kaydı bulunmamaktadır.",
                MessageTitleType.WarningTitle);
            return;
        }

        string reportPath;
        if (string.IsNullOrWhiteSpace(renovation.ReportPath) || !File.Exists(renovation.ReportPath))
        {
            var renovationViewModel = _viewModelFactory.CreateRenovationViewModel(renovation);
            string updatedDate=renovationViewModel.UpdatedDate != null 
                ? renovationViewModel.UpdatedDate.Value.ToString("yyyyMMddHHmmss") 
                : DateTime.Now.ToString("yyyyMMddHHmmss");
            var file = await _dialogService.SaveFilePickerAsync(topLevel,
                "Araç Kabul Raporu",
                $"{renovationViewModel.CustomerName}-{renovationViewModel.Complaint}-{updatedDate}");

            if (file is null || string.IsNullOrWhiteSpace(file.Path.AbsolutePath))
            {
                await _dialogService.OkMessageBox("Rapor kaydedilemedi. Lütfen tekrar deneyin.",
                    MessageTitleType.ErrorTitle);
                return;
            }

            // Using the path from the saved file, generate the PDF
            var report = new RepairReportDocument(renovationViewModel);
            reportPath = Uri.UnescapeDataString(file.Path.AbsolutePath);
            report.GeneratePdf(reportPath);

            _unitOfWork.RenovationsRepository.UpdateRenovationReportPath(renovation.Id, reportPath);
        }
        else
            reportPath = renovation.ReportPath;

        // string userHomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        //
        // // Combine the home directory path with the "Downloads" folder name
        // string downloadsPath = Path.Combine(userHomeDirectory, "Downloads");
        // var firstPdfFile = Directory.EnumerateFiles(downloadsPath, "*.pdf", SearchOption.TopDirectoryOnly)
        //     .FirstOrDefault();

        var pdfViewModel = _viewModelFactory.CreatePdfViewerViewModel(reportPath);
        await _dialogService.OpenPdfViewerWindow(pdfViewModel);
    }

    public async Task SaveChanges()
    {
        await _unitOfWork.SaveChangesAsync();
    }
}