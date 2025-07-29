using CommunityToolkit.Mvvm.ComponentModel;

namespace RepairTracking.ViewModels;

public partial class ForgotPasswordViewModel : ViewModelBase
{
    [ObservableProperty] private int _userId;
    [ObservableProperty] private string _code = string.Empty;
}