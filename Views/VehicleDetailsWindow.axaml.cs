using System;
using System.Linq;
using Avalonia.Controls;

namespace RepairTracking.Views;

public partial class VehicleDetailsWindow : Window
{
    public VehicleDetailsWindow()
    {
        InitializeComponent();
        if (Design.IsDesignMode) return;
        var now = DateTime.Now.Year;
        modelYears.ItemsSource = Enumerable.Range(1980, now - 1980 + 1).ToList();
    }
}