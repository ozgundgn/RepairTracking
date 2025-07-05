using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using ReactiveUI;
using RepairTracking.Services;
using RepairTracking.ViewModels;

namespace RepairTracking.Views;

public partial class CustomerDetailsDialogWindow : ReactiveWindow<CustomerWithAllDetailsViewModel>
{
    public CustomerDetailsDialogWindow()
    {
        InitializeComponent();
        Username.Content = AppServices.UserSessionService.CurrentUser?.Fullname ?? "Unknown User";
        this.WhenActivated(disposables =>
        {
            ViewModel!.OpenEditCustomerDialogWindow.RegisterHandler(DoOpenEditCustomerDialogWindowAsync)
                .DisposeWith(disposables);
            ViewModel!.OpenVehicleDetailsDialogWindow.RegisterHandler(OpenVehicleDetailsDialogWindowAsync)
                .DisposeWith(disposables);
            ViewModel!.OpenRepairDetailsDialogWindow.RegisterHandler(OpenRepairDetailsDialogWindowAsync)
                .DisposeWith(disposables);
        });
    }
    
    private async Task DoOpenEditCustomerDialogWindowAsync(
        IInteractionContext<EditCustomerViewModel, CustomerViewModel?> interaction)
    {
        var window = this.GetVisualRoot() as Window;
        if (window == null)
            return;

        var dialog = new EditCustomerWindow();
        dialog.DataContext = interaction.Input;

        var result = await dialog.ShowDialog<CustomerViewModel?>(window);
        interaction.SetOutput(result);
    }

    private async Task OpenVehicleDetailsDialogWindowAsync(
        IInteractionContext<VehicleDetailsViewModel, Unit> interaction)
    {
        var window = this.GetVisualRoot() as Window;
        if (window == null)
            return;

        var dialog = new VehicleDetailsWindow();
        dialog.DataContext = interaction.Input;

        Unit result = await dialog.ShowDialog<Unit>(window);
        interaction.SetOutput(result);
    }
    
    private async Task OpenRepairDetailsDialogWindowAsync(
        IInteractionContext<SaveRepairDetailViewModel, Unit> interaction)
    {
        var window = this.GetVisualRoot() as Window;
        if (window == null)
            return;

        var dialog = new SaveRepairDetailWindow();
        dialog.DataContext = interaction.Input;

        Unit result = await dialog.ShowDialog<Unit>(window);
        interaction.SetOutput(result);
    }
    
}