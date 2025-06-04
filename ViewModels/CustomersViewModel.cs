using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using RepairTracking.Data;
using RepairTracking.Data.Models;

namespace RepairTracking.ViewModels;

public class CustomersViewModel : ViewModelBase
{
    public class CustomerMainModel 
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PlateNumber { get; set; }
    }
   
   private ObservableCollection<CustomerMainModel> _customersModels;

   public ObservableCollection<CustomerMainModel> CustomersModels
    {
        get => _customersModels;
        set
        {
            SetProperty(ref _customersModels, value);
            OnPropertyChanged();
        }
    }
   
    private ObservableCollection<Customer> _customers;
    private ObservableCollection<Vehicle> _vehicles;
    private ObservableCollection<CustomersVehicle> _customersVehicles;
    private readonly AppDbContext _context;
    public IAsyncRelayCommand LoadDataCommand { get; }

    public ObservableCollection<Customer> Customers
    {
        get => _customers;
        set
        {
            SetProperty(ref _customers, value);
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Vehicle> Vehicles
    {
        get => _vehicles;
        set
        {
            SetProperty(ref _vehicles, value);
            OnPropertyChanged();
        }
    }

    public ObservableCollection<CustomersVehicle> CustomersVehicles
    {
        get => _customersVehicles;
        set
        {
            SetProperty(ref _customersVehicles, value);
            OnPropertyChanged();
        }
    }

    public CustomersViewModel(AppDbContext context)
    {
        _customersModels = new ObservableCollection<CustomerMainModel>();
        _context = context;
        LoadDataCommand = new AsyncRelayCommand(LoadCustomers);
    }

    public async Task LoadCustomers()
    {
        if (_context != null)
        {
            // var customers = await _context.Customers.Where(x=>!x.Passive).ToListAsync();
            var vehicles = await _context.Vehicles
                .Include(c => c.Customer)
                .Where(x => !x.Passive)
                .Where(x => !x.Customer.Passive)
                .Select(x => new CustomerMainModel
                {
                    Name = x.Customer.Name,
                    Surname = x.Customer.Surname,
                    PlateNumber = x.PlateNumber
                }).ToListAsync();
            // Customers = new ObservableCollection<Customer>(customers);
            CustomersModels = new ObservableCollection<CustomerMainModel>(vehicles);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}