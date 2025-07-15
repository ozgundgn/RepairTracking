using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
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
                ? desktop.MainWindow
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
                ? desktop.MainWindow
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
}