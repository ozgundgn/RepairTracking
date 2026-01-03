using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using ReactiveUI;
using RepairTracking.ViewModels;

namespace RepairTracking.Views;

public partial class CustomerDetailsDialogWindow : ReactiveWindow<CustomerWithAllDetailsViewModel>
{
    public CustomerDetailsDialogWindow()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            // ViewModel!.OpenEditCustomerDialogWindow.RegisterHandler(DoOpenEditCustomerDialogWindowAsync)
            //     .DisposeWith(disposables);
            // ViewModel!.OpenVehicleDetailsDialogWindow.RegisterHandler(OpenVehicleDetailsDialogWindowAsync)
            //     .DisposeWith(disposables);
            // ViewModel!.OpenRepairDetailsDialogWindow.RegisterHandler(OpenRepairDetailsDialogWindowAsync)
            //     .DisposeWith(disposables);
        });
        DataContextChanged += OnDataContextChanged;
        
        Opened += (_, _) =>
        {
            var screen = Screens.Primary;

            if (screen is not null)
            {
                Width = Math.Min(Width, screen.WorkingArea.Width);
                Height = Math.Min(Height, screen.WorkingArea.Height);
            }
        };
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is CustomerWithAllDetailsViewModel viewModel)
        {
            viewModel.View = this;
        }
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