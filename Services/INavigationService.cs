using Avalonia.Controls;

namespace RepairTracking.Services;

public interface INavigateService
{
    void NavigateTo<TWindow>() where TWindow : Window, new();
    void CloseCurrent(Window window);
}