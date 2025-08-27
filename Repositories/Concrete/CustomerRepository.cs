using System.Linq;
using System.Text.RegularExpressions;
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

    public async Task<bool> CheckIfCustomerExistsAsync(string phoneNumber, int customerId = 0)
    {
        // Normalize the phone number by removing non-digit characters
        string normalizedPhoneNumber = Regex.Replace(phoneNumber, @"\D", "");

        var query = Context.Customers.AsNoTracking().Where(c => !c.Passive);

        if (customerId > 0)
        {
            query = query.Where(c => c.Id != customerId);
        }

        var customer = await query.FirstOrDefaultAsync(c =>
            c.PhoneNumber == normalizedPhoneNumber);

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

    public async Task DeleteCustomerAsync(int customerId)
    {
        var customer = await Context.Customers
            .Include(x => x.Vehicles)
            .ThenInclude(x => x.Renovations)
            .FirstOrDefaultAsync(c => c.Id == customerId);

        if (customer != null)
        {
            customer.Passive = true;
            Context.Customers.Update(customer);

            if (customer.Vehicles.Count > 0)
            {
                foreach (var vehicle in customer.Vehicles)
                {
                    vehicle.Passive = true;
                    Context.Vehicles.Update(vehicle);

                    if (vehicle.Renovations.Count > 0)
                    {
                        foreach (var renovation in vehicle.Renovations)
                        {
                            renovation.Passive = true;
                            Context.Renovations.Update(renovation);
                        }
                    }
                }
            }

            await Context.SaveChangesAsync();
        }
    }
}