using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairTracking.ViewModels;

namespace RepairTracking.Views;

public partial class FilePickerView : UserControl
{
    public FilePickerView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is FilePickerViewModel viewModel)
        {
            viewModel.GetTopLevel = () => TopLevel.GetTopLevel(this);
        }
    }
}