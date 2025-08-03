using System.Collections.Generic;
using RepairTracking.Data.Models;
using RepairTracking.Models;

namespace RepairTracking.ViewModels.Factories;

public interface IViewModelFactory
{
    VehicleDetailsViewModel CreateVehicleDetailsViewModel(string customerFullName, int? vehicleId = null,
        int? customerId = null);

    AddCustomerViewModel CreateAddCustomerViewModel(IEnumerable<VehicleCustomerModel> existingCustomers);
    CustomerWithAllDetailsViewModel CreateCustomerWithAllDetailsViewModel(int customerId);

    EditCustomerViewModel CreateEditCustomerViewModel(string name, string surname, string? email, string? phoneNumber,
        string? address, int customerId);

    SaveRepairDetailViewModel CreateSaveRepairDetailViewModel(VehicleViewModel selectedVehicleModel,
        int? renovationId = null);

    ChangePasswordViewModel CreateChangePasswordViewModel(string username);

    ForgotPasswordViewModel CreateForgotPasswordViewModel(int userId,string sendedCode,string email);

    LoginViewModel CreateLoginViewModel();

    UserViewModel CreateUserViewModel();

    AddOrUpdateUserViewModel CreateAddOrUpdateUserViewModel(int? userId, string? name, string? surname, string? phone,
        string? username);

    RenovationViewModel CreateRenovationViewModel(Renovation renovation);
    PdfViewerViewModel CreatePdfViewerViewModel(string reportPath);
    DeliveryDateViewModel CreateDeliveryDateViewModel(RenovationViewModel renovationViewModel);
}