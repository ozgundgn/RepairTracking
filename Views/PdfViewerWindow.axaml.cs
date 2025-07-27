using Avalonia.ReactiveUI;
using RepairTracking.ViewModels;

namespace RepairTracking.Views;

public partial class PdfViewerWindow : ReactiveWindow<PdfViewerViewModel>
{
    public PdfViewerWindow()
    {
        InitializeComponent();
    }
}