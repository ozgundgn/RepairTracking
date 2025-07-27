using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using RepairTracking.Repositories.Abstract;
using RepairTracking.ViewModels;
using RepairTracking.ViewModels.Factories;

namespace RepairTracking.Services;

public class NavigationService(MainWindowViewModel mainWindowViewModel, IServiceProvider serviceProvider,IViewModelFactory viewModelFactory)
    : INavigationService
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

    public void NavigateToLogin()
    {
        var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
        var loginVm = viewModelFactory.CreateLoginViewModel();
        mainWindowViewModel.CurrentView = loginVm;
    }

    public void NavigateToHome()
    {
        var homeVm = serviceProvider.GetRequiredService<HomeViewModel>();
        mainWindowViewModel.CurrentView = homeVm;
    }
}