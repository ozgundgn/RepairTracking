using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using RepairTracking.Data.Models;
using RepairTracking.Services;

namespace RepairTracking.ViewModels;

public partial class EditCustomerViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isInValid;

    [ObservableProperty] [Required(ErrorMessage = "Ad alanı boş bırakılamaz.")]
    private string _name;

    [ObservableProperty] [Required(ErrorMessage = "Soyad alanı boş bırakılamaz.")]
    private string _surname;

    [ObservableProperty] [Required(ErrorMessage = "Telefon alanı boş bırakılamaz.")]
    private string _phoneNumber;

    [ObservableProperty] private string? _email;

    [ObservableProperty] private string? _address;

    #region Costumer Validation Properties

    [ObservableProperty] private bool _nameHasError;

    [ObservableProperty] private bool _surnameHasError;

    [ObservableProperty] private bool _phoneNumberHasError;

    public string NameError => GetPropertyErrors(nameof(Name));
    public string SurnameError => GetPropertyErrors(nameof(Surname));
    public string PhoneNumberError => GetPropertyErrors(nameof(PhoneNumber));

    private string GetPropertyErrors(string propertyName)
    {
        var errors = GetErrors(propertyName) as IEnumerable;
        return string.Join(Environment.NewLine,
            errors?.Cast<ValidationResult>().Select(e => e.ErrorMessage) ?? Enumerable.Empty<string>());
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

    #endregion

    public int Id { get; set; }
    
    public CustomerViewModel? ReturnCustomerViewModel()
    {
        return new CustomerViewModel(new Customer()
        {
            Id=Id,
            Surname = Surname,
            PhoneNumber = PhoneNumber,
            Name = Name,
            Email = Email,
            Address = Address
        });
    }
}