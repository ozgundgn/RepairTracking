using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using RepairTracking.Data.Models;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.ViewModels;

public partial class VehicleDetailsViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isInValid;

    [ObservableProperty] [Required(ErrorMessage = "Ad alanı boş bırakılamaz.")]
    private string plateNumber;

    [ObservableProperty] private string? chassisNo;
    [ObservableProperty] private string? engineNo;
    [ObservableProperty] private string? model;
    [ObservableProperty] private string? color;
    [ObservableProperty] private int? km;
    [ObservableProperty] private string? fuel;
    [ObservableProperty] private bool? passive = false;
    [ObservableProperty] private int customerId;
    [ObservableProperty] private string? type;
    [ObservableProperty] private bool _plateNumberHasError;

    public string PlateNumberError => GetPropertyErrors(nameof(PlateNumber));

    private string GetPropertyErrors(string propertyName)
    {
        var errors = GetErrors(propertyName) as IEnumerable;
        return string.Join(Environment.NewLine,
            errors?.Cast<ValidationResult>().Select(e => e.ErrorMessage) ?? Enumerable.Empty<string>());
    }

    partial void OnPlateNumberChanged(string value)
    {
        ValidateProperty(value, nameof(PlateNumber));
        OnPropertyChanged(nameof(PlateNumber));
        IsInValid = PlateNumberHasError = !string.IsNullOrEmpty(PlateNumberError);
    }

    public string _custormerFullname { get; set; }
    private readonly int _vehicleId;

    private readonly IVehicleRepository _repository;
    private readonly ICustomersVehiclesRepository _customersVehiclesRepository;


    public VehicleDetailsViewModel(IVehicleRepository repository, string custormerFullname,
        ICustomersVehiclesRepository customersVehiclesRepository, int? vehicleId = null)
    {
        _repository = repository;
        _custormerFullname = custormerFullname;
        _customersVehiclesRepository = customersVehiclesRepository;
        if (vehicleId != null)
        {
            _vehicleId = vehicleId != null ? vehicleId.Value : 0;
            var vehicle = _repository.GetVehicleByCVehicleId(_vehicleId);
            if (vehicle != null)
            {
                PlateNumber = vehicle.PlateNumber;
                ChassisNo = vehicle.ChassisNo;
                EngineNo = vehicle.EngineNo;
                Model = vehicle.Model.ToString();
                Color = vehicle.Color;
                Km = vehicle.Km;
                Fuel = vehicle.Fuel;
                Passive = vehicle.Passive;
                CustomerId = vehicle.CustomerId;
                Type = vehicle.Type;
            }
        }
    }

    [RelayCommand]
    public async Task AddOrUpdateVehicle()
    {
        bool result = false;
        string message = string.Empty;
        var vehicle = new Vehicle()
        {
            PlateNumber = PlateNumber,
            ChassisNo = ChassisNo,
            EngineNo = EngineNo,
            Model = string.IsNullOrEmpty(Model) ? null : int.Parse(Model),
            Color = Color,
            Km = Km,
            Fuel = Fuel,
            Passive = Passive ?? false,
            CustomerId = CustomerId,
            Type = Type
        };
        if (_vehicleId > 0)
        {
            vehicle.Id = _vehicleId;
            result = await _repository.UpdateVehicle(vehicle);
            await _repository.SaveChangesAsync();
            message = result ? "Kullanıcı başarıyla güncellendi." : "Kullanıcı güncellenirken bir hata oluştu.";
        }
        else
        {
            var pastChassisNoExist = string.Empty;
            var existingVehicle = await _repository.GetVehicleByChassisNo(ChassisNo);
            if (existingVehicle != null)
            {
                if (!existingVehicle.Passive)
                {
                    message = "Bu şasi numarasına sahip bir araç zaten mevcut. Lütfen farklı bir şasi numarası girin.";
                    var activeExist = MessageBoxManager
                        .GetMessageBoxStandard("", message,
                            ButtonEnum.Ok);
                    await activeExist.ShowAsync();
                    return;
                }
                else
                    pastChassisNoExist = " Bu şasi numarasına sahip bir araç daha önce eklenmiş ancak pasif durumda!";
            }

            vehicle.CustomerId = CustomerId;
            var addedVehicle = await _repository.AddVehicle(vehicle);
            await _repository.SaveChangesAsync();
            result = addedVehicle != null;
            message = result
                ? "Kullanıcı başarıyla eklendi." + pastChassisNoExist
                : "Kullanıcı eklerken bir hata oluştu.";

            var CustomerVehicle = new CustomersVehicle()
            {
                CustomerId = customerId,
                VehicleId = addedVehicle.Id,
                CreatedDate = DateTime.Now
            };
            await _customersVehiclesRepository.Add(CustomerVehicle);
            await _customersVehiclesRepository.SaveChangesAsync();
        }

        var addedBox = MessageBoxManager
            .GetMessageBoxStandard("", message,
                ButtonEnum.Ok);

        await addedBox.ShowAsync();
    }
}