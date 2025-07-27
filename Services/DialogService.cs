using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
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

        var result = await dialog.ShowDialog<CustomerViewModel?>(
            App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null);

        return result;
    }

    public async Task<Unit> OpenVehicleDetailsDialogWindow(VehicleDetailsViewModel viewModel)
    {
        var dialog = new VehicleDetailsWindow
        {
            DataContext = viewModel
        };

        var result = await dialog.ShowDialog<Unit>(
            App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.Windows.FirstOrDefault(x => x.Name == "CustomerDetailWindow") ?? desktop.MainWindow
                : null);
        return result;
    }

    public async Task<CustomerViewModel?> OpenEditCustomerDialogWindow(EditCustomerViewModel viewModel)
    {
        var dialog = new EditCustomerWindow()
        {
            DataContext = viewModel
        };
        var result = await dialog.ShowDialog<CustomerViewModel>(
            App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.Windows.FirstOrDefault(x => x.Name == "CustomerDetailWindow")
                : null);
        return result;
    }

    public async Task<Unit> OpenVehicleDetailsDialogWindowAsync(VehicleDetailsViewModel viewModel)
    {
        var dialog = new VehicleDetailsWindow()
        {
            DataContext = viewModel
        };
        var result = await dialog.ShowDialog<Unit>(
            App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null);
        return result;
    }

    public async Task<Unit> OpenCustomerDetailsDialogWindow(CustomerWithAllDetailsViewModel viewModel)
    {
        var dialog = new CustomerDetailsDialogWindow()
        {
            DataContext = viewModel
        };
        var result = await dialog.ShowDialog<Unit>(
            App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null);
        return result;
    }

    public async Task<Unit> OpenRepairDetailsDialogWindow(SaveRepairDetailViewModel viewModel)
    {
        var dialog = new SaveRepairDetailWindow()
        {
            DataContext = viewModel
        };
        var result = await dialog.ShowDialog<Unit>(
            App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.Windows.FirstOrDefault(x => x.Name == "CustomerDetailWindow")
                : null);
        return result;
    }

    public async Task<Unit> OpenChangePasswordDialogWindow(ChangePasswordViewModel viewModel)
    {
        var dialog = new ChangePasswordWindow()
        {
            DataContext = viewModel
        };
        var result = await dialog.ShowDialog<Unit>(
            App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null);
        return result;
    }

    public async Task<Unit> OpenForgotPasswordDialogWindow(ForgotPasswordViewModel viewModel)
    {
        var dialog = new ForgotPasswordWindow()
        {
            DataContext = viewModel
        };
        var result = await dialog.ShowDialog<Unit>(
            App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null);
        return result;
    }

    public async Task<Unit> OpenUsersWindow(UserViewModel viewModel)
    {
        var dialog = new UsersWindow()
        {
            DataContext = viewModel
        };
        var result = await dialog.ShowDialog<Unit>(
            App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null);
        return result;
    }

    public async Task<Unit> OpenAddOrUpdateUserDialogWindow(AddOrUpdateUserViewModel viewModel)
    {
        var dialog = new AddOrUpdateUserWindow
        {
            DataContext = viewModel
        };
        var result = await dialog.ShowDialog<Unit>(
            App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null);
        return result;
    }

    public async Task<Unit> OpenPdfViewerWindow(PdfViewerViewModel viewModel)
    {
        var dialog = new PdfViewerWindow
        {
            DataContext = viewModel
        };
        var result = await dialog.ShowDialog<Unit>(
            App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null);
        return result;
    }

    public async Task<bool> YesNoMessageBox(string message, string title = "Uyarı")
    {
        var warning = MessageBoxManager
            .GetMessageBoxStandard(title,
                message,
                ButtonEnum.YesNo);
        var deleteResult = await warning.ShowAsync();
        return deleteResult == ButtonResult.Yes;
    }

    public async Task OkMessageBox(string message, string title)
    {
        var box = MessageBoxManager
            .GetMessageBoxStandard(title, message);
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
            SuggestedFileName =
                $"Rapor-{fileName}",
            DefaultExtension = "pdf",
            FileTypeChoices = [new FilePickerFileType("PDF Document") { Patterns = ["*.pdf"] }]
        });
        return file;
    }
}