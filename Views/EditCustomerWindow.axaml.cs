using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using RepairTracking.ViewModels;

namespace RepairTracking.Views;

public partial class EditCustomerWindow : Window
{
    public EditCustomerWindow()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is EditCustomerViewModel vm)
        {
            Close(vm.ReturnCustomerViewModel()); // Return result to Interaction
        }
    }
}