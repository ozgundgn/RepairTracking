using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairTracking.Data;
using RepairTracking.Data.Models;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.Repositories.Concrete;

public class CustomerRepository(AppDbContext context) : BaseContext(context), ICustomerRepository
{
    public async Task<Customer> AddAsync(Customer customer)
    {
        var entity = await Context.Customers.AddAsync(customer);
        return entity.Entity;
    }

    public async Task<bool> UpdateAsync(int id, Customer customer)
    {
        var entity = await Context.Customers.Where(x => x.Id == id).ExecuteUpdateAsync(c => c
            .SetProperty(x => x.Name, customer.Name)
            .SetProperty(x => x.Surname, customer.Surname)
            .SetProperty(x => x.PhoneNumber, customer.PhoneNumber)
            .SetProperty(x => x.Email, customer.Email)
            .SetProperty(x => x.Address, customer.Address));
        return entity > 0;
    }

    public async Task<bool> CheckIfCustomerExistsAsync(string name, string surname)
    {
        var customer = await Context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Name == name && c.Surname == surname);
        return customer != null;
    }

    public Customer? GetCustomerWithAllDetails(int customerId)
    {
        return Context.Customers
            .AsNoTracking()
            .Include(x => x.CreatedUserNavigation)
            .Include(c => c.Vehicles)
            .ThenInclude(v => v.Renovations.Where(reno => reno.Passive != true))
            .ThenInclude(r => r.RenovationDetails)
            .FirstOrDefault(c => c.Id == customerId);
    }
}