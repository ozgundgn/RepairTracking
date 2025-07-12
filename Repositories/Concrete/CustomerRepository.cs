using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairTracking.Data;
using RepairTracking.Data.Models;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.Repositories.Concrete;

public class CustomerRepository : BaseContext, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        var entity = await context.Customers.AddAsync(customer);
        return entity.Entity;
    }

    public async Task<bool> UpdateAsync(int id, Customer customer)
    {
        var entity = await context.Customers.Where(x => x.Id == id).ExecuteUpdateAsync(c => c
            .SetProperty(x => x.Name, customer.Name)
            .SetProperty(x => x.Surname, customer.Surname)
            .SetProperty(x => x.PhoneNumber, customer.PhoneNumber)
            .SetProperty(x => x.Email, customer.Email)
            .SetProperty(x => x.Address, customer.Address));
        return entity > 0;
    }

    public Customer? GetCustomerWithAllDetails(int customerId)
    {
        return context.Customers
            .Include(x => x.CreatedUserNavigation)
            .Include(c => c.Vehicles)
            .ThenInclude(v => v.Renovations.Where(reno => reno.Passive != true))
            .ThenInclude(r => r.RenovationDetails)
            .FirstOrDefault(c => c.Id == customerId);
    }
}