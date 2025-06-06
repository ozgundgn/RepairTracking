using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RepairTracking.Models;

public class VehicleCustomerModel : INotifyPropertyChanged
{
    public int CustomerId { get; set; }
    public int VehicleId { get; set; }
    private string _name, _surname, _plateNumber;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public string Surname
    {
        get => _surname;
        set
        {
            _surname = value;
            OnPropertyChanged();
        }
    }

    public string PlateNumber
    {
        get => _plateNumber;
        set
        {
            _plateNumber = value;
            OnPropertyChanged();
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}