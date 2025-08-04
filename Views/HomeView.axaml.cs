using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using ReactiveUI;
using RepairTracking.Models;
using RepairTracking.ViewModels;

namespace RepairTracking.Views;

public partial class HomeView : ReactiveUserControl<HomeViewModel>
{
    // private HomeViewModel? ViewModel => DataContext as HomeViewModel;
    public HomeView()
    {
        InitializeComponent();
        // Option 1: Resolve ViewModel here if not set globally in App.axaml.cs
        // This is good if each window manages its own ViewModel lifecycle
        // var app = (App)Application.Current;
        // if (app is { Services: not null })
        // {
        //     DataContext = app.Services.GetRequiredService<CustomersViewModel>();
        // }
        // DataContext = viewModel; // Set the ViewModel passed in the constructor
        // viewModel?.LoadDataCommand.Execute(this);
        this.WhenActivated(disposables =>
        {
            // ViewModel?.OpenAddCustomerDialogWindow.RegisterHandler(DoOpenAddCustomerDialogWindowAsync)
            //     .DisposeWith(disposables);
            // ViewModel?.OpenVehicleDetailsDialogWindow.RegisterHandler(OpenVehicleDetailsDialogWindowAsync)
            //     .DisposeWith(disposables);
            // ViewModel?.OpenCustomerDetailsDialogWindow.RegisterHandler(OpenCustomerDetailsDialogWindowAsync)
            //     .DisposeWith(disposables);
        });
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is HomeViewModel viewModel)
        {
            viewModel.GetTopLevel = () => TopLevel.GetTopLevel(this);
        }
    }


    private async Task DoOpenAddCustomerDialogWindowAsync(
        IInteractionContext<AddCustomerViewModel, CustomerViewModel?> interaction)
    {
        var window = this.GetVisualRoot() as Window;
        if (window == null)
            return;

        var dialog = new AddCustomerWindow
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<CustomerViewModel?>(window);
        interaction.SetOutput(result);
    }

    private async Task OpenVehicleDetailsDialogWindowAsync(
        IInteractionContext<VehicleDetailsViewModel, Unit> interaction)
    {
        var window = this.GetVisualRoot() as Window;
        if (window == null)
            return;

        var dialog = new VehicleDetailsWindow
        {
            DataContext = interaction.Input
        };

        Unit result = await dialog.ShowDialog<Unit>(window);
        interaction.SetOutput(result);
    }

    private async Task OpenCustomerDetailsDialogWindowAsync(
        IInteractionContext<CustomerWithAllDetailsViewModel, Unit> interaction)
    {
        var window = this.GetVisualRoot() as Window;
        if (window == null)
            return;

        var dialog = new CustomerDetailsDialogWindow
        {
            DataContext = interaction.Input
        };

        Unit result = await dialog.ShowDialog<Unit>(window);
        interaction.SetOutput(result);
    }

    private async void DataGrid_OnCellEditEnded(object? sender, DataGridCellEditEndedEventArgs e)
    {
        var column = (e.Column as DataGridTextColumn)?.Header?.ToString();

        if (DataContext is HomeViewModel vm && e.Row.DataContext is VehicleCustomerModel item)
        {
            if (column == "Plaka")
            {
                await vm.UpdatePlateNumber(item.VehicleId, item.PlateNumber);
                await vm.SaveChanges();
            }
        }
    }
}