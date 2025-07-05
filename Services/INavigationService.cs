using Avalonia.Controls;
using RepairTracking.Models;

namespace RepairTracking.Services;

public interface INavigationService
{
    void NavigateTo<TWindow>() where TWindow : Window, new();
    void CloseCurrent(Window window);
    void NavigateToLogin();
    void NavigateToHome();
    void NavigateToRepairDetail(VehicleCustomerModel repairDetail);
}