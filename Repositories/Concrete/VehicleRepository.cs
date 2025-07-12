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
using RepairTracking.ViewModels;

namespace RepairTracking.Repositories.Concrete;

public class VehicleRepository(AppDbContext context) : BaseContext(context), IVehicleRepository
{
    public async Task<List<Vehicle>> GetVehicleByCustomerId(int customerId)
    {
        return await context.Vehicles.Where(x => x.Customer.Id == customerId).ToListAsync();
    }

    public async Task<Vehicle?> GetVehicleByChassisNo(string chassisNo)
    {
        return await context.Vehicles.FirstOrDefaultAsync(x => x.ChassisNo == chassisNo);
    }

    public async Task<List<int>> GetPassiveVehicleIdsByChassisNo(string chassisNo)
    {
        var list = await context.Vehicles.Where(x => x.ChassisNo == chassisNo && x.Passive).ToListAsync();
        return list.Select(x => x.Id).ToList();
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

    public bool DeleteVehicle(int id)
    {
        var updateVehicleResult = context.Vehicles.Where(v => v.Id == id)
            .ExecuteUpdate(x => x.SetProperty(pr => pr.Passive, true));
        var updateRenovationResult = context.Renovations.Where(v => v.VehicleId == id)
            .ExecuteUpdate(r => r.SetProperty(x => x.Passive, true));
        return (updateVehicleResult > 0 && updateRenovationResult > 0);
    }

    public async Task DeleteCustomerAsync(int customerId)
    {
        await context.Database.ExecuteSqlRawAsync("EXEC DeactivateCustomer @CustomerId = {0}", customerId);
    }

    public async Task<List<VehicleCustomerModel>> GetVehicleCustomerModelAsync(int? vehicleId)
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
                PhoneNumber = x.Customer.PhoneNumber,
                VehicleId = x.Id,
                Type = x.Type,
                ChassisNo = x.ChassisNo,
                Model = x.Model,
                CreatedUser = x.Customer.CreatedUserNavigation.Name + " " + x.Customer.CreatedUserNavigation.Surname,
            }).ToListAsync();
    }

    public List<VehicleCustomerModel> GetVehicleCustomerModel(int? vehicleId = null)
    {
        Expression<Func<Vehicle, bool>> predicate = x => !x.Passive && !x.Customer.Passive;

        if (vehicleId != null)
            predicate.And(x => x.Id == vehicleId);

        return context.Vehicles
            .Include(c => c.Customer)
            .Where(predicate)
            .Select(x => new VehicleCustomerModel
            {
                Name = x.Customer.Name,
                Surname = x.Customer.Surname,
                PlateNumber = x.PlateNumber,
                CustomerId = x.Customer.Id,
                PhoneNumber = x.Customer.PhoneNumber,
                VehicleId = x.Id,
                Type = x.Type,
                ChassisNo = x.ChassisNo,
                Model = x.Model,
                CreatedUser = x.Customer.CreatedUserNavigation.Name + " " + x.Customer.CreatedUserNavigation.Surname,
            }).ToList();
    }

    public Vehicle? GetVehicleByCVehicleId(int vehcileId)
    {
        return context.Vehicles
            .Find(vehcileId);
    }

    public List<Vehicle>? GetAllVehicleByChassises(IEnumerable<string>? chassisNos)
    {
        if (chassisNos != null)
            return context.Vehicles.Where(x => chassisNos.Contains(x.ChassisNo))
                .ToList();
        return null;
    }
}