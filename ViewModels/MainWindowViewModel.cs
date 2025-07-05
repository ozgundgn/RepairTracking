using CommunityToolkit.Mvvm.ComponentModel;

namespace RepairTracking.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentView;
}