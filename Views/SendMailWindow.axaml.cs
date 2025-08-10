using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairTracking.ViewModels;

namespace RepairTracking.Views;

public partial class SendMailWindow : Window
{
    public SendMailWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is SendMailViewModel vm)
        {
            vm.View = this;
        }
    }
}