using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using RepairTracking.Repositories.Abstract;
using RepairTracking.ViewModels;
using RepairTracking.ViewModels.Factories;

namespace RepairTracking.Services;

public class NavigationService(
    MainWindowViewModel mainWindowViewModel,
    IViewModelFactory viewModelFactory)
    : INavigationService
{
    public void NavigateTo<TWindow>() where TWindow : Window, new()
    {
        var window = new TWindow();
        window.Show();
    }

    public void NavigateToLogin()
    {
        var loginVm = viewModelFactory.CreateLoginViewModel();
        mainWindowViewModel.CurrentView = loginVm;
    }

    public void NavigateToHome()
    {
        var homeVm = viewModelFactory.CreateHomeViewModel();
        mainWindowViewModel.CurrentView = homeVm;
    }
}