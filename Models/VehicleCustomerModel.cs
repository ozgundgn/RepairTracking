using CommunityToolkit.Mvvm.ComponentModel;
using RepairTracking.ViewModels;

namespace RepairTracking.Models;

public partial class VehicleCustomerModel : ViewModelBase
{
    public int CustomerId { get; set; }
    public int VehicleId { get; set; }
    public string Name { get; set; }
    public string? Type { get; set; }
    public string? ChassisNo { get; set; }
    public int? Model { get; set; }
    public string Surname { get; set; }
    public string PlateNumber { get; set; }
    public string PhoneNumber { get; set; }
    public string CreatedUser { get; set; }
    [ObservableProperty] private bool _isSelected;
}