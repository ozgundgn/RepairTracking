using System.Threading.Tasks;
using RepairTracking.Data.Models;

namespace RepairTracking.Repositories.Abstract;

public interface ICustomerRepository : IBaseContext
{
    Task<Customer> AddAsync(Customer email);
    Task<bool> UpdateAsync(int id,Customer customer);
    Customer? GetCustomerWithAllDetails(int customerId);
    Task<bool> CheckIfCustomerExistsAsync(string name, string surname);
}