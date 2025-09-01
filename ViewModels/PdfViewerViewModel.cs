using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PDFtoImage;
using RepairTracking.Data.Models;
using RepairTracking.Helpers;
using RepairTracking.Services;
using SkiaSharp;

namespace RepairTracking.ViewModels;

public partial class PdfViewerViewModel : ViewModelBase
{
    [ObservableProperty] private string _reportPath;
    [ObservableProperty] private string _filePath = string.Empty;
    [ObservableProperty] private Renovation? _renovation;

    private readonly IDialogService _dialogService;

    // Store the loaded PDF as a byte array in memory
    private byte[]? _pdfBytes;

    [ObservableProperty] private Bitmap? _pdfPageImage; // Bound to the Image control in the View

    [ObservableProperty] private int _pageCount;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    private int _currentPage = 0;

    [ObservableProperty] private string _pageInfo = "No PDF Loaded";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    private bool _isPdfLoaded;

    // --- NEW ZOOM PROPERTIES ---

    // This property will be bound to the ScaleTransform in the View.
    // We use a partial method OnZoomFactorChanged to update the text property.
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ZoomOutCommand))]
    [NotifyCanExecuteChangedFor(nameof(ResetZoomCommand))]
    private double _zoomFactor = 1.0;

    // This property is for displaying "120%" in the UI.
    [ObservableProperty] private string _zoomPercentageText = "100%";

    // This method is called automatically by the CommunityToolkit source generator
    // whenever the ZoomFactor property changes.
    partial void OnZoomFactorChanged(double value)
    {
        // Format the double as a percentage with zero decimal places (e.g., 1.2 -> "120%")
        ZoomPercentageText = $"{value:P0}";
    }

    // --- Unchanged Methods (LoadPdfFromPath, RenderPage, Pagination) ---
    // (These are omitted for brevity but should remain in your file)

    // --- NEW ZOOM COMMANDS ---

    [RelayCommand]
    private void ZoomIn()
    {
        // Increase zoom by 10%
        ZoomFactor += 0.1;
    }

    [RelayCommand(CanExecute = nameof(CanZoomOut))]
    private void ZoomOut()
    {
        // Decrease zoom by 10%
        ZoomFactor -= 0.1;
    }

    private bool CanZoomOut()
    {
        // Prevent zooming out to be smaller than 20%
        return ZoomFactor > 0.2;
    }

    public bool CanResetZoom => ZoomFactor != 1.0;

    [RelayCommand(CanExecute = nameof(CanResetZoom))]
    private void ResetZoom()
    {
        ZoomFactor = 1.0;
    }

    public PdfViewerViewModel(string reportPath, IDialogService dialogService)
    {
        _reportPath = reportPath;
        _dialogService = dialogService;
        LoadPdfFromPath();
    }

    private void LoadPdfFromPath()
    {
        if (string.IsNullOrEmpty(ReportPath) || !File.Exists(ReportPath))
            PageInfo = "File not found or path is invalid.";

        IsPdfLoaded = false;
        PdfPageImage = null;
        _pdfBytes = null;

        try
        {
            _pdfBytes = File.ReadAllBytes(ReportPath);
            PageCount = Conversion.GetPageCount(_pdfBytes);
            IsPdfLoaded = true;
            CurrentPage = 0;
            RenderPage();
        }
        catch (Exception ex)
        {
            PageInfo = $"Error loading PDF: {ex.Message}";
        }
    }

    private void RenderPage()
    {
        if (_pdfBytes == null) return;

        // 1. PDFtoImage renders the PDF page into an SKBitmap object.
        //    We must wrap this in a 'using' block to ensure the bitmap's memory is properly released.
        using SKBitmap skBitmap = Conversion.ToImage(_pdfBytes, page: CurrentPage);

        // 2. The SKBitmap cannot be used directly by Avalonia. We must encode it
        //    into a standard image format (like PNG) and write it to a memory stream.
        using (var memoryStream = new MemoryStream())
        {
            // 3. Encode the bitmap's data as a PNG and save it to the stream.
            skBitmap.Encode(memoryStream, SKEncodedImageFormat.Png, 100);

            // 4. Rewind the stream's position to the beginning.
            memoryStream.Position = 0;

            // 5. Now, create the Avalonia Bitmap from the prepared stream.
            //    This will work correctly.
            PdfPageImage = new Bitmap(memoryStream);
        }

        // Update the page info text
        PageInfo = $"{CurrentPage + 1} / {PageCount}";
    }

    [RelayCommand]
    private void PreviousPage()
    {
        CurrentPage--;
        CheckCanGoForBothSide();
        RenderPage();
    }

    private bool _canGoToPreviousPage;
    private bool _canGoToNextPage;

    public bool CanGoToPreviousPage
    {
        get => PageCount > 1 && CurrentPage + 1 > 1;
        set => SetProperty(ref _canGoToPreviousPage, value);
    }

    public bool CanGoToNextPage
    {
        get => PageCount > 1 && CurrentPage + 1 < PageCount;
        set => SetProperty(ref _canGoToNextPage, value);
    }

    private void CheckCanGoForBothSide()
    {
        CanGoToPreviousPage = PageCount > 1 && CurrentPage + 1 > 1;
        CanGoToNextPage = PageCount > 1 && CurrentPage + 1 < PageCount;
    }

    [RelayCommand]
    private void NextPage()
    {
        CurrentPage++;
        CheckCanGoForBothSide();
        RenderPage();
    }

    [RelayCommand]
    private async Task Print()
    {
        var success = await PlatformPrintService.PrintFile(ReportPath);
        if (success)
            await _dialogService.OkMessageBox("Rapor yazıcıya gönderildi.", MessageTitleType.SuccessTitle);
        else
            await _dialogService.OkMessageBox("Rapor yazıcıya gönderilirken bir sorun oluştu.",
                MessageTitleType.ErrorTitle);
    }

    private void PrintOnWindows(string filePath)
    {
        var printProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = filePath,
                Verb = "print",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            }
        };

        printProcess.Start();
    }

    private void PrintOnLinux(string filePath)
    {
        var printProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "lpr",
                Arguments = $"\"{filePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        printProcess.Start();
    }

    private void PrintOnMac(string filePath)
    {
        var printProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "lpr",
                Arguments = $"\"{filePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        printProcess.Start();
    }
}