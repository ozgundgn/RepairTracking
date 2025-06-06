using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using RepairTracking.Models;
using RepairTracking.ViewModels;

namespace RepairTracking.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel? viewModel)
    {
        InitializeComponent();
        // Option 1: Resolve ViewModel here if not set globally in App.axaml.cs
        // This is good if each window manages its own ViewModel lifecycle
        // var app = (App)Application.Current;
        // if (app is { Services: not null })
        // {
        //     DataContext = app.Services.GetRequiredService<CustomersViewModel>();
        // }
        DataContext = viewModel; // Set the ViewModel passed in the constructor
        // viewModel?.LoadDataCommand.Execute(this);
    }
}