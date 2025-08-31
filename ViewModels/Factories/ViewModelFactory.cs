using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RepairTracking.Data.Models;
using RepairTracking.Models;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;

namespace RepairTracking.ViewModels.Factories;

public class ViewModelFactory(IUnitOfWork unitOfWork, IDialogService dialogService) : IViewModelFactory
{
    public VehicleDetailsViewModel CreateVehicleDetailsViewModel(string customerFullName, int? vehicleId = null,
        int? customerId = null)
    {
        return new VehicleDetailsViewModel(unitOfWork, dialogService, vehicleId, customerId)
        {
            CustormerFullname = customerFullName,
            VehicleId = vehicleId,
            CustomerId = customerId
        };
    }

    public AddCustomerViewModel CreateAddCustomerViewModel(IEnumerable<VehicleCustomerModel> existingCustomers)
    {
        return new AddCustomerViewModel(unitOfWork)
        {
            ExistingCustomers = existingCustomers.ToList()
        };
    }

    public CustomerWithAllDetailsViewModel CreateCustomerWithAllDetailsViewModel(int customerId)
    {
        return new CustomerWithAllDetailsViewModel(unitOfWork, dialogService, this, customerId);
    }

    public EditCustomerViewModel CreateEditCustomerViewModel(string name, string surname, string? email,
        string? phoneNumber,
        string? address, int customerId)
    {
        return new EditCustomerViewModel()
        {
            Name = name,
            Surname = surname,
            Email = email,
            PhoneNumber = phoneNumber,
            Address = address,
            Id = customerId
        };
    }

    public SaveRepairDetailViewModel CreateSaveRepairDetailViewModel(VehicleViewModel selectedVehicleModel,
        int? renovationId = null)
    {
        return new SaveRepairDetailViewModel(unitOfWork, selectedVehicleModel, dialogService, renovationId);
    }

    public ChangePasswordViewModel CreateChangePasswordViewModel(string username)
    {
        return new ChangePasswordViewModel(unitOfWork.UsersRepository, dialogService)
        {
            Username = username
        };
    }

    public ForgotPasswordViewModel CreateForgotPasswordViewModel(int userId, string sendedCode, string email)
    {
        return new ForgotPasswordViewModel(unitOfWork.UsersRepository, dialogService)
        {
            UserId = userId,
            SendedCode = sendedCode,
            Email = email
        };
    }

    public LoginViewModel CreateLoginViewModel()
    {
        return new LoginViewModel(unitOfWork.UsersRepository, dialogService, this);
    }

    public HomeViewModel CreateHomeViewModel()
    {
        return new HomeViewModel(unitOfWork, this, dialogService);
    }

    public UserViewModel CreateUserViewModel()
    {
        return new UserViewModel(unitOfWork.UsersRepository, dialogService, this);
    }

    public SendMailViewModel CreateSendMailViewModel(string email = "")
    {
        return new SendMailViewModel(dialogService, unitOfWork.MailRepository, unitOfWork.UsersRepository)
        {
            ToEmail = email
        };
    }

    public AddOrUpdateUserViewModel CreateAddOrUpdateUserViewModel(int? userId, string? name, string? surname,
        string? phone,
        string? username)
    {
        return new AddOrUpdateUserViewModel(unitOfWork.UsersRepository, dialogService)
        {
            Name = name,
            Surname = surname,
            Phone = phone,
            Username = username,
            UserId = userId
        };
    }

    public RenovationViewModel CreateRenovationViewModel(Renovation renovation)
    {
        return new RenovationViewModel()
        {
            Id = renovation.Id,
            VehicleId = renovation.VehicleId,
            RepairDate = renovation.RepairDate,
            DeliveryDate = renovation.DeliveryDate,
            Complaint = renovation.Complaint,
            Note = renovation.Note,
            Passive = renovation.Passive,
            RenovationDetails = new ObservableCollection<RenovationDetailViewModel>(renovation.RenovationDetails
                .Select(rd => new RenovationDetailViewModel
                {
                    Id = rd.Id,
                    Description = rd.Description,
                    Price = rd.Price,
                    TCode = rd.TCode,
                    Note = rd.Note,
                    RenovationId = rd.RenovationId,
                    Name = rd.Name
                }).ToList()),
            Vehicle = new VehicleViewModel
            {
                Id = renovation.Vehicle?.Id ?? 0,
                PlateNumber = renovation.Vehicle?.PlateNumber ?? string.Empty,
                Model = renovation.Vehicle?.Model ?? 0,
                ChassisNo = renovation.Vehicle?.ChassisNo,
                EngineNo = renovation.Vehicle?.EngineNo,
                CustomerId = renovation.Vehicle?.CustomerId ?? 0,
                Fuel = renovation.Vehicle?.Fuel,
                Type = renovation.Vehicle?.Type,
                Color = renovation.Vehicle?.Color,
                Km = renovation.Vehicle?.Km ?? 0,
                Passive = renovation.Vehicle?.Passive ?? false
            },
            CustomerName = renovation.Vehicle?.Customer?.Name ?? string.Empty,
            CustomerSurname = renovation.Vehicle?.Customer?.Surname ?? string.Empty,
            PhoneNumber = renovation.Vehicle?.Customer?.PhoneNumber ?? string.Empty,
            Email = renovation.Vehicle?.Customer?.Email ?? string.Empty,
            Address = renovation.Vehicle?.Customer?.Address ?? string.Empty,
            CreatedDate = renovation.CreatedDate,
            UpdatedDate = renovation.UpdatedDate
        };
    }

    public PdfViewerViewModel CreatePdfViewerViewModel(string reportPath) => new(reportPath);

    public DeliveryDateViewModel CreateDeliveryDateViewModel(RenovationViewModel renovationViewModel)
    {
        return new DeliveryDateViewModel(unitOfWork.RenovationsRepository, dialogService)
        {
            RenovationViewModel = renovationViewModel
        };
    }
}