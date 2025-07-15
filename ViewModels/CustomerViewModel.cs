using RepairTracking.Data.Models;

namespace RepairTracking.ViewModels;

public class CustomerViewModel(Customer customer) : ViewModelBase
{
   public Customer Customer = customer;
}