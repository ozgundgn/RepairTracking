using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairTracking.Data;
using RepairTracking.Data.Models;
using RepairTracking.Helpers;
using RepairTracking.Models;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.Repositories.Concrete;

public class VehicleRepository : BaseContext, IVehicleRepository
{
    public VehicleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Vehicle>> GetVehicleByCustomerId(int customerId)
    {
        return await context.Vehicles.Where(x => x.Customer.Id == customerId).ToListAsync();
    }

    public async Task<Vehicle> AddVehicle(Vehicle vehicle)
    {
        var entity = await context.Vehicles.AddAsync(vehicle);
        return entity.Entity;
    }

    public async Task<bool> UpdateVehicle(Vehicle vehicle)
    {
        return await context.Vehicles.Where(x => x.Id == vehicle.Id).ExecuteUpdateAsync(
            x => x
                .SetProperty(v => v.Km, vehicle.Km)
                .SetProperty(v => v.Fuel, vehicle.Fuel)
                .SetProperty(v => v.Passive, vehicle.Passive)
                .SetProperty(v => v.PlateNumber, vehicle.PlateNumber)
                .SetProperty(v => v.Type, vehicle.Type)
                .SetProperty(v => v.ChassisNo, vehicle.ChassisNo)
                .SetProperty(v => v.EngineNo, vehicle.EngineNo)
                .SetProperty(v => v.Model, vehicle.Model)
                .SetProperty(v => v.Color, vehicle.Color)
                .SetProperty(v => v.CustomerId, vehicle.CustomerId)) > 0;
    }

    public async Task<bool> UpdatePlateNumber(int vehicleId, string plateNumber)
    {
        return await context.Vehicles
            .Where(x => x.Id == vehicleId)
            .ExecuteUpdateAsync(x => x.SetProperty(v => v.PlateNumber, plateNumber)) > 0;
    }

    public async Task<bool> DeleteVehicle(int id)
    {
        return await context.Vehicles.Where(x => x.Id == id).ExecuteDeleteAsync() > 0;
    }

    public async Task<List<VehicleCustomerModel>> GetVehicleCustomerModel(int? vehicleId)
    {
        Expression<Func<Vehicle, bool>> predicate = x => !x.Passive && !x.Customer.Passive;

        if (vehicleId != null)
            predicate.And(x => x.Id == vehicleId);

        return await context.Vehicles
            .Include(c => c.Customer)
            .Where(predicate)
            .Select(x => new VehicleCustomerModel
            {
                Name = x.Customer.Name,
                Surname = x.Customer.Surname,
                PlateNumber = x.PlateNumber,
                CustomerId = x.Customer.Id,
                VehicleId = x.Id,
            }).ToListAsync();
    }
}