using Avalonia.Controls;
using RepairTracking.Models;

namespace RepairTracking.Services;

public interface INavigationService
{
    void NavigateTo<TWindow>() where TWindow : Window, new();
    void NavigateToLogin();
    void NavigateToHome();
}