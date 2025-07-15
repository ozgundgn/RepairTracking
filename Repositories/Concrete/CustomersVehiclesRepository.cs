using System.Threading.Tasks;
using RepairTracking.Data;
using RepairTracking.Data.Models;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.Repositories.Concrete;

public class CustomersVehiclesRepository : BaseContext, ICustomersVehiclesRepository
{
    public CustomersVehiclesRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> Add(CustomersVehicle customerVehicle)
    {
        var entity = await Context.CustomersVehicles.AddAsync(customerVehicle);
        return entity.State == Microsoft.EntityFrameworkCore.EntityState.Added;
    }
}