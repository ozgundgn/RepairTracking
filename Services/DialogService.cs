using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using RepairTracking.ViewModels;
using RepairTracking.Views;

namespace RepairTracking.Services;

public class DialogService : IDialogService
{
    public async Task<CustomerViewModel?> OpenAddCustomerDialogAsync(AddCustomerViewModel viewModel)
    {
        var dialog = new AddCustomerWindow
        {
            DataContext = viewModel
        };

        Window? owner = WindowLocator.GetActiveWindow();
        var result = await dialog.ShowDialog<CustomerViewModel?>(owner);
        return result;
    }

    public async Task<Unit> OpenVehicleDetailsDialogWindow(VehicleDetailsViewModel viewModel)
    {
        var dialog = new VehicleDetailsWindow
        {
            DataContext = viewModel
        };
        Window? owner = WindowLocator.GetActiveWindow();
        var result = await dialog.ShowDialog<Unit>(owner);
        return result;
    }

    public async Task<CustomerViewModel?> OpenEditCustomerDialogWindow(EditCustomerViewModel viewModel)
    {
        var dialog = new EditCustomerWindow()
        {
            DataContext = viewModel
        };
        Window? owner = WindowLocator.GetActiveWindow();

        var result = await dialog.ShowDialog<CustomerViewModel>(owner);
        return result;
    }

    public async Task<Unit> OpenVehicleDetailsDialogWindowAsync(VehicleDetailsViewModel viewModel)
    {
        var dialog = new VehicleDetailsWindow()
        {
            DataContext = viewModel
        };
        Window? owner = WindowLocator.GetActiveWindow();

        var result = await dialog.ShowDialog<Unit>(owner);
        return result;
    }

    public async Task<Unit> OpenCustomerDetailsDialogWindow(CustomerWithAllDetailsViewModel viewModel)
    {
        var dialog = new CustomerDetailsDialogWindow()
        {
            DataContext = viewModel
        };
        Window? owner = WindowLocator.GetActiveWindow();
        var result = await dialog.ShowDialog<Unit>(owner);
        return result;
    }

    public async Task<Unit> OpenRepairDetailsDialogWindow(SaveRepairDetailViewModel viewModel)
    {
        var dialog = new SaveRepairDetailWindow()
        {
            DataContext = viewModel
        };
        Window? owner = WindowLocator.GetActiveWindow();
        var result = await dialog.ShowDialog<Unit>(owner);
        return result;
    }

    public async Task<Unit> OpenChangePasswordDialogWindow(ChangePasswordViewModel viewModel)
    {
        var dialog = new ChangePasswordWindow()
        {
            DataContext = viewModel
        };
        Window? owner = WindowLocator.GetActiveWindow();

        var result = await dialog.ShowDialog<Unit>(owner);
        return result;
    }

    public async Task<Unit> OpenForgotPasswordDialogWindow(ForgotPasswordViewModel viewModel)
    {
        var dialog = new ForgotPasswordWindow
        {
            DataContext = viewModel
        };
        Window? owner = WindowLocator.GetActiveWindow();

        var result = await dialog.ShowDialog<Unit>(owner);
        return result;
    }

    public async Task<Unit> OpenUsersWindow(UserViewModel viewModel)
    {
        var dialog = new UsersWindow()
        {
            DataContext = viewModel
        };
        Window? owner = WindowLocator.GetActiveWindow();
        var result = await dialog.ShowDialog<Unit>(owner);
        return result;
    }

    public async Task<Unit> OpenSendMailWindow(SendMailViewModel viewModel)
    {
        var dialog = new SendMailWindow
        {
            DataContext = viewModel
        };
        Window? owner = WindowLocator.GetActiveWindow();

        var result = await dialog.ShowDialog<Unit>(owner);
        return result;
    }

    public async Task<Unit> OpenAddOrUpdateUserDialogWindow(AddOrUpdateUserViewModel viewModel)
    {
        var dialog = new AddOrUpdateUserWindow
        {
            DataContext = viewModel
        };
        Window? owner = WindowLocator.GetActiveWindow();

        var result = await dialog.ShowDialog<Unit>(owner);
        return result;
    }

    public async Task<Unit> OpenPdfViewerWindow(PdfViewerViewModel viewModel)
    {
        var dialog = new PdfViewerWindow
        {
            DataContext = viewModel
        };
        Window? owner = WindowLocator.GetActiveWindow();
        var result = await dialog.ShowDialog<Unit>(owner);
        return result;
    }

    public async Task<Unit> OpenDeliveryDateDialogWindow(DeliveryDateViewModel viewModel)
    {
        var dialog = new DeliveriyDateWindow()
        {
            DataContext = viewModel
        };
        Window? owner = WindowLocator.GetActiveWindow();
        var result = await dialog.ShowDialog<Unit>(owner);
        return result;
    }

    public async Task<bool> YesNoMessageBox(string message, string title = "Uyarı")
    {
        var warning = MessageBoxManager
            .GetMessageBoxStandard(title,
                message,
                ButtonEnum.YesNo);
        Window? owner = WindowLocator.GetActiveWindow();

        var deleteResult = await warning.ShowWindowDialogAsync(owner);
        return deleteResult == ButtonResult.Yes;
    }

    public async Task OkMessageBox(string message, string title)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(title, message);
        Window? owner = WindowLocator.GetActiveWindow();

        if (owner != null)
        {
            await box.ShowWindowDialogAsync(owner);
            return;
        }

        await box.ShowAsync();
    }

    public async Task<IStorageFile?> SaveFilePickerAsync(TopLevel topLevel, string title, string fileName)
    {
        // 2. Open the "Save File" dialog
        if (topLevel.StorageProvider is not { } storageProvider)
            return null;

        var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = $"Rapor-{fileName}",
            DefaultExtension = "pdf",
            FileTypeChoices = [new FilePickerFileType("PDF Document") { Patterns = ["*.pdf"] }]
        });
        return file;
    }

    public void CloseCurrentWindow()
    {
        Window? owner = WindowLocator.GetActiveWindow();
        if (owner is not null)
            owner.Close();
    }
}