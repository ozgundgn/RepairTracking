using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Media.TextFormatting.Unicode;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Models;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    private VehicleCustomerModel SelectedCustomer;

    private ObservableCollection<VehicleCustomerModel> _customersModels;
    private readonly IVehicleRepository _repository;
    public IAsyncRelayCommand LoadDataCommand { get; }

    public ObservableCollection<VehicleCustomerModel> CustomersModels
    {
        get => _customersModels;
        set
        {
            SetProperty(ref _customersModels, value);
            OnPropertyChanged();
        }
    }

    public HomeViewModel(IVehicleRepository repository, MainWindowViewModel mainWindowViewModel)
    {
        if (!AppState.Instance.IsAuthenticated)
            _mainWindowViewModel.NavigateToLogin();

        _customersModels = new ObservableCollection<VehicleCustomerModel>();
        _repository = repository;
        _mainWindowViewModel = mainWindowViewModel;
        LoadDataCommand = new AsyncRelayCommand(LoadCustomers);
        if (SelectedCustomer != null)
            _mainWindowViewModel.NavigateToRepairDetail(SelectedCustomer);
    }

    public async Task LoadCustomers()
    {
        // var customers = await _context.Customers.Where(x=>!x.Passive).ToListAsync();
        var vehicles = await _repository.GetVehicleCustomerModel();
        CustomersModels = new ObservableCollection<VehicleCustomerModel>(vehicles);
    }

    public async Task UpdatePlateNumber(int vehicleId, string plateNumber)
    {
        await _repository.UpdatePlateNumber(vehicleId, plateNumber);
    }

    public async Task SaveChanges()
    {
        // Assuming you have a method to save changes in your repository
        await _repository.SaveChangesAsync();
    }
}