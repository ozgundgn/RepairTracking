using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
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

    [ObservableProperty] [Required(ErrorMessage = "Plaka alanı boş bırakılamaz.")]
    private string _plateNumber;

    [ObservableProperty] [Required(ErrorMessage = "Şasi no alanı boş bırakılamaz.")]
    private string? _chassisNo;

    [ObservableProperty] private string? _engineNo;
    [ObservableProperty] private string? _model;
    [ObservableProperty] private string? _color;
    [ObservableProperty] private int? _km;
    [ObservableProperty] private string? _fuel;
    [ObservableProperty] private bool? _passive = false;
    [ObservableProperty] private int? _customerId;
    [ObservableProperty] private string? _type;
    [ObservableProperty] private byte[]? _image;
    [ObservableProperty] private bool _plateNumberHasError;
    [ObservableProperty] private FilePickerViewModel _filePickerViewModel;
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

    public VehicleDetailsViewModel(IUnitOfWork unitOfWork, IDialogService dialogService, int? vehicleId,
        int? customerId)
    {
        _unitOfWork = unitOfWork;
        _dialogService = dialogService;
        _repository = _unitOfWork.VehiclesRepository;
        _customersVehiclesRepository = _unitOfWork.CustomersVehiclesRepository;
        _renovationRepository = _unitOfWork.RenovationsRepository;

        Vehicle? registeredVehicle = null;
        if (vehicleId != null)
        {
             registeredVehicle = _repository.GetVehicleByCVehicleId(vehicleId.Value);
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
        FilePickerViewModel = new FilePickerViewModel(registeredVehicle?.Image)
        {
            PickingButtonText = "Araç Resmi Seç",
            FilePickerTypes = [FilePickerFileTypes.ImageAll],
            SelectedImageData = registeredVehicle?.Image
        };
    }

    [RelayCommand]
    private async Task AddOrUpdateVehicle()
    {
        ValidateAllProperties();
        if (HasErrors) return;
        bool result;
        string message;

        var checkIfVehicleExists =
            await _repository.GetVehicleByChassisNo(ChassisNo, VehicleId > 0 ? VehicleId.Value : 0);

        if (checkIfVehicleExists != null && !checkIfVehicleExists.Passive)
        {
            await _dialogService.OkMessageBox(
                "Bu şasi numarasına sahip bir araç zaten mevcut. Lütfen farklı bir şasi numarası girin.",
                MessageTitleType.WarningTitle);
            return;
        }

        var checkIfPlateNumberExists =
            await _repository.GetVehicleByPlateNumber(PlateNumber.ToUpper(), VehicleId > 0 ? VehicleId.Value : 0);
        if (checkIfPlateNumberExists != null)
        {
            await _dialogService.OkMessageBox(
                "Bu plaka numarasına sahip bir araç zaten mevcut. Lütfen farklı bir plaka numarası girin.",
                MessageTitleType.WarningTitle);
            return;
        }

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
            CustomerId = CustomerId ?? 0,
            Type = Type,
        };
        
        if (FilePickerViewModel != null)
            vehicle.Image = FilePickerViewModel.SelectedImageData;

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
            if (checkIfVehicleExists != null && checkIfVehicleExists.Passive)
                pastChassisNoExist = " Bu şasi numarasına sahip bir araç daha önce eklenmiş ancak pasif durumda!";
            
            var addedVehicle = await _repository.AddVehicle(vehicle);
            await _unitOfWork.SaveChangesAsync();
            result = addedVehicle != null;
            message = result
                ? "Araç bilgisi başarıyla eklendi." + pastChassisNoExist
                : "Araç bilgisi eklenirken bir hata oluştu.";

            if (addedVehicle != null)
            {
                var customerVehicle = new CustomersVehicle()
                {
                    CustomerId = CustomerId ?? 0,
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