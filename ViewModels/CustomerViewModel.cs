using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using RepairTracking.Data;
using RepairTracking.Data.Models;
using RepairTracking.Models;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Repositories.Concrete;

namespace RepairTracking.ViewModels;

public class CustomersViewModel : ViewModelBase
{
    private ObservableCollection<VehicleCustomerModel> _customersModels;

    public ObservableCollection<VehicleCustomerModel> CustomersModels
    {
        get => _customersModels;
        set
        {
            SetProperty(ref _customersModels, value);
            OnPropertyChanged();
        }
    }

    private readonly IVehicleRepository _repository;
    public IAsyncRelayCommand LoadDataCommand { get; }

    public CustomersViewModel(IVehicleRepository repository)
    {
        _customersModels = new ObservableCollection<VehicleCustomerModel>();
        _repository = repository;
        LoadDataCommand = new AsyncRelayCommand(LoadCustomers);
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