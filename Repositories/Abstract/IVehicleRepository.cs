using System.Collections.Generic;
using System.Threading.Tasks;
using RepairTracking.Data.Models;
using RepairTracking.Models;

namespace RepairTracking.Repositories.Abstract;

public interface IVehicleRepository:IBaseContext
{
    Task<List<Vehicle>> GetVehicleByCustomerId(int customerId);
    Task<Vehicle> AddVehicle(Vehicle vehicle);
    Task<bool> UpdateVehicle(Vehicle vehicle);
    Task<bool> UpdatePlateNumber(int vehicleId, string plateNumber);
    
    Task<bool> DeleteVehicle(int id);

    Task<List<VehicleCustomerModel>> GetVehicleCustomerModel(int? vehicleId = null);
}