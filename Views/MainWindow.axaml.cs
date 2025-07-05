using Avalonia.ReactiveUI;
using RepairTracking.ViewModels;

namespace RepairTracking.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    // private MainWindowViewModel? ViewModel => DataContext as MainWindowViewModel;

    public MainWindow()
    {
        InitializeComponent();
    }

    // This code is only valid in newer ReactiveUI which is shipped since avalonia 11.2.0 
}