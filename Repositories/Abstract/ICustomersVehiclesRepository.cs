using System.Threading.Tasks;
using RepairTracking.Data.Models;

namespace RepairTracking.Repositories.Abstract;

public interface ICustomersVehiclesRepository:IBaseContext
{
    Task<bool> Add(CustomersVehicle customerVehicle);
}