using System.Collections.Generic;
using System.Threading.Tasks;
using RepairTracking.Data.Models;
using RepairTracking.Models;

namespace RepairTracking.Repositories.Abstract;

public interface IVehicleRepository : IBaseContext
{
    Task<List<Vehicle>> GetVehicleByCustomerId(int customerId);
    Task<Vehicle?> AddVehicle(Vehicle vehicle);
    Task<bool> UpdateVehicle(Vehicle vehicle);
    Task<bool> UpdatePlateNumber(int vehicleId, string plateNumber);
    Task DeleteVehicle(int id);
    Task<List<VehicleCustomerModel>> GetVehicleCustomerModelAsync(int? vehicleId = null);
    List<VehicleCustomerModel> GetVehicleCustomerModel(int? vehicleId = null);
    Vehicle? GetVehicleByCVehicleId(int vehcileId);
    List<Vehicle>? GetAllVehicleByChassises(IEnumerable<string?> chassisNos);
    Task<Vehicle?> GetVehicleByChassisNo(string chassisNo,int vehicleId = 0);
    Task<List<int>> GetPassiveVehicleIdsByChassisNo(string chassisNo);
    Task<Vehicle?> GetVehicleByPlateNumber(string plateNumber, int vehicleId = 0);
}