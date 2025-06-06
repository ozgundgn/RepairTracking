using Avalonia.Controls;
using Avalonia.Interactivity;
using RepairTracking.Models;
using RepairTracking.ViewModels;

namespace RepairTracking.Views;

public partial class HomeView : UserControl
{
    private HomeViewModel? ViewModel => DataContext as HomeViewModel;

    public HomeView()
    {
        InitializeComponent();
        AttachedToVisualTree += (_, _) =>
        {
            var vm = ViewModel;
            if (vm != null)
            {
                vm.LoadDataCommand.Execute(this);
            }
        };
       
    }
    public HomeView(HomeViewModel? viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.LoadDataCommand.Execute(this);
        // Option 1: Resolve ViewModel here if not set globally in App.axaml.cs
        // This is good if each window manages its own ViewModel lifecycle
        // var app = (App)Application.Current;
        // if (app is { Services: not null })
        // {
        //     DataContext = app.Services.GetRequiredService<CustomersViewModel>();
        // }
        // DataContext = viewModel; // Set the ViewModel passed in the constructor
        // viewModel?.LoadDataCommand.Execute(this);
    }

    private async void LoadButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            await ViewModel.LoadCustomers();
        }
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