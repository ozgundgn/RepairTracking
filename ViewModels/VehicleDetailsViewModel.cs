using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Data.Models;
using RepairTracking.Helpers;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;

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
            errors?.Cast<ValidationResult>().Select(e => e.ErrorMessage) ?? []);
    }

    partial void OnPlateNumberChanged(string value)
    {
        ValidateProperty(value, nameof(PlateNumber));
        OnPropertyChanged(nameof(PlateNumber));
        IsInValid = PlateNumberHasError = !string.IsNullOrEmpty(PlateNumberError);
    }

    [ObservableProperty] private string _custormerFullname;
    [ObservableProperty] private int? _vehicleId;

    private readonly IVehicleRepository _repository;
    private readonly IRenovationRepository _renovationRepository;
    private readonly ICustomersVehiclesRepository _customersVehiclesRepository;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IDialogService _dialogService;

    public VehicleDetailsViewModel(IUnitOfWork unitOfWork, IDialogService dialogService, int? vehicleId)
    {
        _unitOfWork = unitOfWork;
        _dialogService = dialogService;
        _repository = _unitOfWork.VehiclesRepository;
        _customersVehiclesRepository = _unitOfWork.CustomersVehiclesRepository;
        _renovationRepository = _unitOfWork.RenovationsRepository;
        if (vehicleId != null)
        {
            var registeredVehicle = _repository.GetVehicleByCVehicleId(vehicleId.Value);
            if (registeredVehicle != null)
            {
                VehicleId = registeredVehicle.Id;
                PlateNumber = registeredVehicle.PlateNumber;
                ChassisNo = registeredVehicle.ChassisNo;
                EngineNo = registeredVehicle.EngineNo;
                Model = registeredVehicle.Model.ToString();
                Color = registeredVehicle.Color;
                Km = registeredVehicle.Km;
                Fuel = registeredVehicle.Fuel;
                Passive = registeredVehicle.Passive;
                CustomerId = registeredVehicle.CustomerId;
                Type = registeredVehicle.Type;
            }
        }
    }

    [RelayCommand]
    private async Task AddOrUpdateVehicle()
    {
        bool result;
        string message;
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
        if (VehicleId > 0)
        {
            vehicle.Id = VehicleId.Value;
            result = await _repository.UpdateVehicle(vehicle);

            if (Passive == true)
                _renovationRepository.PassiveRenovation(VehicleId.Value);

            await _unitOfWork.SaveChangesAsync();
            message = result ? "Araç bilgisi başarıyla güncellendi." : "Araç bilgisi güncellenirken bir hata oluştu.";
        }
        else
        {
            var pastChassisNoExist = string.Empty;
            var existingVehicle = await _repository.GetVehicleByChassisNo(ChassisNo);
            if (existingVehicle != null)
            {
                if (!existingVehicle.Passive)
                {
                    await _dialogService.OkMessageBox(
                        "Bu şasi numarasına sahip bir araç zaten mevcut. Lütfen farklı bir şasi numarası girin.",
                        MessageTitleType.WarningTitle);
                    return;
                }

                pastChassisNoExist = " Bu şasi numarasına sahip bir araç daha önce eklenmiş ancak pasif durumda!";
            }

            vehicle.CustomerId = CustomerId;
            var addedVehicle = await _repository.AddVehicle(vehicle);
            await _unitOfWork.SaveChangesAsync();
            result = addedVehicle != null;
            message = result
                ? "Araç bilgisi başarıyla eklendi." + pastChassisNoExist
                : "Ara bilgisi eklenirken bir hata oluştu.";

            if (addedVehicle != null)
            {
                var customerVehicle = new CustomersVehicle()
                {
                    CustomerId = CustomerId,
                    VehicleId = addedVehicle.Id,
                    CreatedDate = DateTime.Now
                };
                await _customersVehiclesRepository.Add(customerVehicle);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        await _dialogService.OkMessageBox(message, "");
    }
}