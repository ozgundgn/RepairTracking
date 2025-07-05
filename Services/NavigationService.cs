using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using RepairTracking.Models;
using RepairTracking.Repositories.Abstract;
using RepairTracking.ViewModels;

namespace RepairTracking.Services;

public class NavigationService : INavigationService
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(MainWindowViewModel mainWindowViewModel, IServiceProvider serviceProvider)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<TWindow>() where TWindow : Window, new()
    {
        var window = new TWindow();
        window.Show();
    }

    public void CloseCurrent(Window window)
    {
        window.Close();
    }

    public void NavigateToLogin()
    {
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var loginVm = new LoginViewModel(userRepository);
        _mainWindowViewModel.CurrentView = loginVm;
    }

    public void NavigateToHome()
    {
        var homeVm = _serviceProvider.GetRequiredService<HomeViewModel>();
        _mainWindowViewModel.CurrentView = homeVm;
    }

    public void NavigateToRepairDetail(VehicleCustomerModel repairDetail)
    {
        var repairVm = _serviceProvider.GetRequiredService<RepairDetailViewModel>();
        repairVm.SetCar(repairDetail);
        _mainWindowViewModel.CurrentView = repairVm;
    }
}