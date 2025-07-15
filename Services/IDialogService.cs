using System.Reactive;
using System.Threading.Tasks;
using RepairTracking.ViewModels;

namespace RepairTracking.Services;

public interface IDialogService
{
    Task<CustomerViewModel?> OpenAddCustomerDialogAsync(AddCustomerViewModel viewModel);
    Task<Unit> OpenVehicleDetailsDialogWindow(VehicleDetailsViewModel viewModel);
    Task<CustomerViewModel?> OpenEditCustomerDialogWindow(EditCustomerViewModel viewModel);

    Task<Unit> OpenVehicleDetailsDialogWindowAsync(VehicleDetailsViewModel viewModel);
    Task<Unit> OpenCustomerDetailsDialogWindow(CustomerWithAllDetailsViewModel viewModel);

    Task<Unit> OpenRepairDetailsDialogWindow(SaveRepairDetailViewModel viewModel);
    Task<bool> YesNoMessageBox(string message, string title);
    Task OkMessageBox(string message, string title);
}