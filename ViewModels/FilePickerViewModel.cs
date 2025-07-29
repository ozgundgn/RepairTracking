using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RepairTracking.ViewModels;

public partial class FilePickerViewModel : ViewModelBase
{
    [ObservableProperty] private string _pickingButtonText = "Resim Seç";
    [ObservableProperty] private Bitmap _imageSource;
    [ObservableProperty] private string _selectedImageFileName;
    [ObservableProperty] private Byte[]? _selectedImageData;

    [ObservableProperty]
    private ObservableCollection<FilePickerFileType> _filePickerTypes = [FilePickerFileTypes.ImageAll];

    public FilePickerViewModel(byte[]? image = null)
    {
        if (image is not null)
            ImageSource = new Bitmap(stream: new MemoryStream(image));
    }

    public Func<TopLevel?>? GetTopLevel { get; set; }

    [RelayCommand]
    private async Task LoadFile()
    {
        if (GetTopLevel?.Invoke() is not { } topLevel) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = PickingButtonText,
            AllowMultiple = false,
            FileTypeFilter = FilePickerTypes
        });

        if (files.Count >= 1)
        {
            var file = files[0];
            await using var stream = await file.OpenReadAsync();

            // Store for saving later
            SelectedImageData = await ReadStreamToBytesAsync(stream);
            SelectedImageFileName = file.Name;

            // Rewind stream and create Bitmap to show preview NOW
            stream.Position = 0;
            ImageSource = new Bitmap(stream);
        }
    }

    private async Task<byte[]> ReadStreamToBytesAsync(Stream input)
    {
        using var ms = new MemoryStream();
        await input.CopyToAsync(ms);
        return ms.ToArray();
    }
}