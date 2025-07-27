using CommunityToolkit.Mvvm.ComponentModel;

namespace RepairTracking.ViewModels;

public partial class ForgotPasswordViewModel : ViewModelBase
{
    [ObservableProperty] private string _phone;
}