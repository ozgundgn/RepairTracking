using System.Collections.Generic;
using System.Linq;
using RepairTracking.Models;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;

namespace RepairTracking.ViewModels.Factories;

public class ViewModelFactory(IUnitOfWork unitOfWork, IDialogService dialogService) : IViewModelFactory
{
    public VehicleDetailsViewModel CreateVehicleDetailsViewModel(string customerFullName, int? vehicleId = null)
    {
        return new VehicleDetailsViewModel(unitOfWork,dialogService, vehicleId)
        {
            CustormerFullname = customerFullName,
            VehicleId = vehicleId
        };
    }

    public AddCustomerViewModel CreateAddCustomerViewModel(IEnumerable<VehicleCustomerModel> existingCustomers)
    {
        return new AddCustomerViewModel(unitOfWork)
        {
            ExistingCustomers = existingCustomers.ToList()
        };
    }

    public CustomerWithAllDetailsViewModel CreateCustomerWithAllDetailsViewModel(int customerId)
    {
        return new CustomerWithAllDetailsViewModel(unitOfWork, dialogService, this, customerId);
    }

    public EditCustomerViewModel CreateEditCustomerViewModel(string name, string surname, string? email,
        string? phoneNumber,
        string? address, int customerId)
    {
        return new EditCustomerViewModel()
        {
            Name = name,
            Surname = surname,
            Email = email,
            PhoneNumber = phoneNumber,
            Address = address,
            Id = customerId
        };
    }

    public SaveRepairDetailViewModel CreateSaveRepairDetailViewModel(VehicleViewModel selectedVehicleModel,
        int? renovationId = null)
    {
        return new SaveRepairDetailViewModel(unitOfWork, selectedVehicleModel, dialogService, renovationId);
    }
}