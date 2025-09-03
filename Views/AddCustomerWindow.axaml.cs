using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using MsBox.Avalonia;
using RepairTracking.Services;
using RepairTracking.ViewModels;

namespace RepairTracking.Views;

public partial class AddCustomerWindow : ReactiveWindow<AddCustomerViewModel>
{
    public AddCustomerWindow()
    {
        // This line is ESSENTIAL. If it's missing, the window will fail to load.
        InitializeComponent();
        if (Design.IsDesignMode) return;
        var now = DateTime.Now.Year;
        years.ItemsSource = Enumerable.Range(1980, now - 1980 + 1).ToList();
        // this.WhenActivated(action => action(ViewModel!.SaveCustomerCommand.Subscribe(Close)));
        AuthorizedTextBlock.Text = AppServices.UserSessionService.CurrentUser?.Fullname ?? " Unknown User";
    }

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AddCustomerViewModel vm)
        {
            bool isExist = await vm.ValidateCustomerNotExist();
            if (!isExist)
            {
                var dialogResult = vm.ReturnCustomerViewModel();
                if (dialogResult == null)
                    return;
                Close(dialogResult); // Return result to Interaction
                var addedBox = MessageBoxManager
                    .GetMessageBoxStandard("İşlem Başarılı", "Kullanıcı başarıyla eklendi.");
                await addedBox.ShowAsync();
                Close();
                return;
            }

            var box = MessageBoxManager
                .GetMessageBoxStandard("Uyarı",
                    "Bu telefona ait kullanıcı ya da şaşi numarasına kayıtlı araç zaten mevcut.");

            await box.ShowAsync();
        }
    }
}