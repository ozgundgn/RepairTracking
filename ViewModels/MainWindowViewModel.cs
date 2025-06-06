using System;
using Microsoft.Extensions.DependencyInjection;
using RepairTracking.Models;

namespace RepairTracking.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    private ViewModelBase _currentView;

    public ViewModelBase CurrentView
    {
        get => _currentView;
        set
        {
            SetProperty(ref _currentView, value);
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        if (!AppState.Instance.IsAuthenticated)
            CurrentView = new LoginViewModel(this);
    }

    public void NavigateToLogin()
    {
        var loginVm = _serviceProvider.GetRequiredService<LoginViewModel>();
        CurrentView = loginVm;
    }

    public void NavigateToHome()
    {
        var homeVm = _serviceProvider.GetRequiredService<HomeViewModel>();
        CurrentView = homeVm;
    }

    public void NavigateToRepairDetail(VehicleCustomerModel repairDetail)
    {
        var repairVm = _serviceProvider.GetRequiredService<RepairDetailViewModel>();
        repairVm.SetCar(repairDetail);
        CurrentView = repairVm;
    }
}