using RepairTracking.Models;

namespace RepairTracking.ViewModels;

public class RepairDetailViewModel : ViewModelBase
{
    private VehicleCustomerModel _repairDetail;
    public void SetCar(VehicleCustomerModel repairDetail)
    {
        _repairDetail = repairDetail;
        OnPropertyChanged(nameof(RepairDetail));
    }
}