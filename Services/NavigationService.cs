using Avalonia.Controls;

namespace RepairTracking.Services;

public class NavigateService : INavigateService
{
    public void NavigateTo<TWindow>() where TWindow : Window, new()
    {
        var window = new TWindow();
        window.Show();
    }

    public void CloseCurrent(Window window)
    {
       window.Close();
    }
}