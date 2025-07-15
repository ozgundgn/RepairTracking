using System.Collections.Generic;
using RepairTracking.Models;

namespace RepairTracking.ViewModels.Factories;

public interface IViewModelFactory
{
    VehicleDetailsViewModel CreateVehicleDetailsViewModel(string customerFullName, int? vehicleId = null);
    AddCustomerViewModel CreateAddCustomerViewModel(IEnumerable<VehicleCustomerModel> existingCustomers);
    CustomerWithAllDetailsViewModel CreateCustomerWithAllDetailsViewModel(int customerId);

    EditCustomerViewModel CreateEditCustomerViewModel(string name, string surname, string? email, string? phoneNumber,
        string? address, int customerId);

    SaveRepairDetailViewModel CreateSaveRepairDetailViewModel(VehicleViewModel selectedVehicleModel,
        int? renovationId = null);
}