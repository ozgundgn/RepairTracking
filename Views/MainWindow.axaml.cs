using System;
using Avalonia.ReactiveUI;
using RepairTracking.ViewModels;

namespace RepairTracking.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    // private MainWindowViewModel? ViewModel => DataContext as MainWindowViewModel;

    public MainWindow()
    {
        InitializeComponent();

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

    // This code is only valid in newer ReactiveUI which is shipped since avalonia 11.2.0 
}