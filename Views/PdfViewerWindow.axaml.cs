using System;
using Avalonia.ReactiveUI;
using RepairTracking.ViewModels;

namespace RepairTracking.Views;

public partial class PdfViewerWindow : ReactiveWindow<PdfViewerViewModel>
{
    public PdfViewerWindow()
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
}