using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Data.Models;
using RepairTracking.Models;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;

namespace RepairTracking.ViewModels;

public partial class AddCustomerViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isInValid;

    [ObservableProperty] [Required(ErrorMessage = "Ad alanı boş bırakılamaz.")]
    private string _name;

    [ObservableProperty] [Required(ErrorMessage = "Soyad alanı boş bırakılamaz.")]
    private string _surname;

    [ObservableProperty] [Required(ErrorMessage = "Telefon alanı boş bırakılamaz."),Phone(ErrorMessage = "Telefon formatı geçersiz.")]
    private string _phoneNumber;

    [ObservableProperty]
    [EmailAddress(ErrorMessage = "E-posta formatı geçersiz.")]
    private string _email;

    #region Costumer Validation Properties

    [ObservableProperty] private bool _nameHasError;

    [ObservableProperty] private bool _surnameHasError;

    [ObservableProperty] private bool _phoneNumberHasError;

    [ObservableProperty] private bool _emailHasError;

    public string NameError => GetPropertyErrors(nameof(Name));
    public string SurnameError => GetPropertyErrors(nameof(Surname));
    public string PhoneNumberError => GetPropertyErrors(nameof(PhoneNumber));
    public string EmailError => GetPropertyErrors(nameof(Email));

    private string GetPropertyErrors(string propertyName)
    {
        var errors = GetErrors(propertyName) as IEnumerable;
        return string.Join(Environment.NewLine,
            errors.Cast<ValidationResult>().Select(e => e.ErrorMessage));
    }

    partial void OnNameChanged(string value)
    {
        ValidateProperty(value, nameof(Name));
        OnPropertyChanged(nameof(NameError));
        IsInValid = NameHasError = !string.IsNullOrEmpty(NameError);
    }

    partial void OnSurnameChanged(string value)
    {
        ValidateProperty(value, nameof(Surname));
        OnPropertyChanged(nameof(SurnameError));
        IsInValid = SurnameHasError = !string.IsNullOrEmpty(SurnameError);
    }

    partial void OnPhoneNumberChanged(string value)
    {
        ValidateProperty(value, nameof(PhoneNumber));
        OnPropertyChanged(nameof(PhoneNumberError));
        IsInValid = PhoneNumberHasError = !string.IsNullOrEmpty(PhoneNumberError);
    }

    partial void OnEmailChanged(string value)
    {
        ValidateProperty(value, nameof(Email));
        OnPropertyChanged(nameof(EmailError));
        IsInValid = EmailHasError = !string.IsNullOrEmpty(EmailError);
    }

    #endregion

    [ObservableProperty] [Required(ErrorMessage = "Plaka alanı boş bırakılamaz.")]
    private string _plateNumber;

    [ObservableProperty] [Required(ErrorMessage = "Marka alanı boş bırakılamaz.")]
    private string _brand;

    [ObservableProperty] [Required(ErrorMessage = "Model alanı boş bırakılamaz.")]
    private int _model;

    [ObservableProperty]
    private string _chassisNumber;

    [ObservableProperty] private bool _plateNumberHasError;
    [ObservableProperty] private bool _brandHasError;
    [ObservableProperty] private bool _modelHasError;
    [ObservableProperty] private bool _chassisNumberHasError;

    public string PlateNumberError => GetPropertyErrors(nameof(PlateNumber));
    public string BrandError => GetPropertyErrors(nameof(Brand));
    public string ModelError => GetPropertyErrors(nameof(Model));
    public string ChassisError => GetPropertyErrors(nameof(ChassisNumber));

    partial void OnPlateNumberChanged(string value)
    {
        ValidateProperty(value, nameof(PlateNumber));
        OnPropertyChanged(nameof(PlateNumberError));
        IsInValid = PlateNumberHasError = !string.IsNullOrEmpty(PlateNumberError);
    }

    partial void OnBrandChanged(string value)
    {
        ValidateProperty(value, nameof(Brand));
        OnPropertyChanged(nameof(BrandError));
        IsInValid = BrandHasError = !string.IsNullOrEmpty(BrandError);
    }

    partial void OnModelChanged(int value)
    {
        ValidateProperty(value, nameof(Model));
        OnPropertyChanged(nameof(ModelError));
        IsInValid = ModelHasError = !string.IsNullOrEmpty(ModelError);
    }

    // partial void OnChassisNumberChanged(string value)
    // {
    //     ValidateProperty(value, nameof(ChassisNumber));
    //     OnPropertyChanged(nameof(ChassisError));
    //     IsInValid = ChassisNumberHasError = !string.IsNullOrEmpty(ChassisError);
    // }

    // public ReactiveCommand<Unit, CustomerViewModel?> SaveCustomerCommand { get; }
  
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDialogService _dialogService;

    public AddCustomerViewModel(IUnitOfWork unitOfWork,IDialogService dialogService)
    {
        _unitOfWork = unitOfWork;
        _dialogService = dialogService;
        IsInValid = true;
    }

    // The Save command will close the window and return the new customer

    public async Task<bool> ValidateCustomerNotExist()
    {
        ValidateAllProperties();
        if (HasErrors) return false;
        var customerExists = await _unitOfWork.CustomersRepository.CheckIfCustomerExistsAsync(PhoneNumber);
        if (customerExists)
            return true;

        if (string.IsNullOrWhiteSpace(ChassisNumber)) return false;
        var vehicleExists = await _unitOfWork.VehiclesRepository.GetVehicleByChassisNo(ChassisNumber);
        return vehicleExists != null;
    }

    [RelayCommand]
    private void CloseWindow()
    {
        _dialogService.CloseCurrentWindow();
    }
    public CustomerViewModel? ReturnCustomerViewModel()
    {
        ValidateAllProperties();
        if (HasErrors) return null;
        return new CustomerViewModel(new Customer()
        {
            Surname = Surname,
            PhoneNumber = PhoneNumber,
            Name = Name,
            Email = Email,
            CreatedUser = AppServices.UserSessionService.CurrentUser?.Id ?? 0,
            Vehicles = new List<Vehicle>
            {
                new()
                {
                    PlateNumber = PlateNumber,
                    Type = Brand,
                    Model = Model,
                    Passive = false,
                    ChassisNo = ChassisNumber
                }
            }
        });
    }
}