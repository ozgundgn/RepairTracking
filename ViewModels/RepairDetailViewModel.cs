using CommunityToolkit.Mvvm.ComponentModel;
using RepairTracking.Models;

namespace RepairTracking.ViewModels;

public partial class RepairDetailViewModel : ViewModelBase
{
    private VehicleCustomerModel _repairDetail;
    public void SetCar(VehicleCustomerModel repairDetail)
    {
        _repairDetail = repairDetail;
        OnPropertyChanged(nameof(RepairDetail));
    }
}