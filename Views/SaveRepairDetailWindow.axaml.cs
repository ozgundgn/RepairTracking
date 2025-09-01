using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace RepairTracking.Views;

public partial class SaveRepairDetailWindow : Window
{
    public SaveRepairDetailWindow()
    {
        InitializeComponent();
    }

    private void RepairDateClear_Button_OnClick(object? sender, RoutedEventArgs e)
    {

        RepairDatePicker.SelectedDate = null;
    }
    private void DeliveryDateClear_Button_OnClick(object? sender, RoutedEventArgs e)
    {

        DeliveryDatePicker.SelectedDate = null;
    }
    
}