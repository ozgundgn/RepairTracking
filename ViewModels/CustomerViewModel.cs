using RepairTracking.Data.Models;

namespace RepairTracking.ViewModels;

public class CustomerViewModel : ViewModelBase
{
   public Customer Customer;
   public CustomerViewModel(Customer customer)
   {
      Customer = customer;
   }
   // public string Name => _customer.Name;
   // public string Email => _customer.Email;
   // public string Surname => _customer.Surname;
   // public string PhoneNumber => _customer.PhoneNumber;
   // public string Address => _customer.Address;
   
}