using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using RepairTracking.ViewModels;

namespace RepairTracking.Services;

public interface IDialogService
{
    protected Window? CurrentWindow { get; set; }
    Task<CustomerViewModel?> OpenAddCustomerDialogAsync(AddCustomerViewModel viewModel);
    Task<Unit> OpenVehicleDetailsDialogWindow(VehicleDetailsViewModel viewModel);
    Task<CustomerViewModel?> OpenEditCustomerDialogWindow(EditCustomerViewModel viewModel);

    Task<Unit> OpenVehicleDetailsDialogWindowAsync(VehicleDetailsViewModel viewModel);
    Task<Unit> OpenCustomerDetailsDialogWindow(CustomerWithAllDetailsViewModel viewModel);

    Task<Unit> OpenRepairDetailsDialogWindow(SaveRepairDetailViewModel viewModel);
    Task<Unit> OpenChangePasswordDialogWindow(ChangePasswordViewModel viewModel);
    Task<Unit> OpenForgotPasswordDialogWindow(ForgotPasswordViewModel viewModel);
    Task<Unit> OpenUsersWindow(UserViewModel viewModel);
    Task<Unit> OpenAddOrUpdateUserDialogWindow(AddOrUpdateUserViewModel viewModel);
    Task<Unit> OpenPdfViewerWindow(PdfViewerViewModel viewModel);
    Task<Unit> OpenDeliveryDateDialogWindow(DeliveryDateViewModel viewModel);
    Task<Unit> OpenSendMailWindow(SendMailViewModel viewModel);
    void CloseCurrentWindow();
    Task<bool> YesNoMessageBox(string message, string title);
    Task OkMessageBox(string message, string title);
    Task<IStorageFile?> SaveFilePickerAsync(TopLevel topLevel, string title, string fileName);
}