using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RepairTracking.Data;
using RepairTracking.Data.Models;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.Repositories.Concrete;

public class RenovationRepository(AppDbContext context) : BaseContext(context), IRenovationRepository
{
    public bool AddRenovation(Renovation renovation)
    {
        var entity = Context.Renovations.Add(renovation);
        return entity.State == Microsoft.EntityFrameworkCore.EntityState.Added;
    }

    public bool UpdateRenovation(Renovation renovation)
    {
        var entity = Context.Renovations.Update(renovation);
        return entity.State == EntityState.Modified;
    }

    public bool UpdateRenovationDeliveryDate(int id, DateTime datetime)
    {
        var entity = Context.Renovations.Find(id);
        // If the entity is null or already has a delivery date return 
        if (entity == null)
            return false;
        entity.DeliveryDate = datetime;
        var result = Context.Renovations.Update(entity);
        return result.State == EntityState.Modified;
    }

    public bool DeleteRenovation(int id)
    {
        var result = Context.Renovations.Where(x => x.Id == id)
            .ExecuteUpdate(sr => sr.SetProperty(c => c.Passive, true));
        return result > 0;
    }

    public bool PassiveRenovation(int vehicleId)
    {
        var result = Context.Renovations.AsNoTracking().Where(x => x.VehicleId == vehicleId)
            .ExecuteUpdate(sr => sr.SetProperty(c => c.Passive, true));
        return result > 0;
    }

    public Renovation? GetRenovationById(int id)
    {
        var entity = Context.Renovations
            .AsNoTracking()
            .Include(r => r.Vehicle)
            .ThenInclude(v => v.Customer)
            .Include(r => r.RenovationDetails)
            .FirstOrDefault(r => r.Id == id);
        return entity;
    }

    public List<Renovation> GetRenovationsByVehcileIds(int[] ids)
    {
        var renovationList = Context.Renovations.Where(x => ids.Contains(x.VehicleId))
            .Include(r => r.Vehicle)
            .ThenInclude(v => v.Customer)
            .Include(r => r.RenovationDetails)
            .ToList();
        return renovationList;
    }

    public bool DeleteRenovationDetails(List<RenovationDetail> renovations)
    {
        Context.RenovationDetails
            .RemoveRange(renovations);
        return Context.SaveChanges() > 0;
    }

    public bool AddRenovationDetail(RenovationDetail renovationDetail)
    {
        var entity = Context.RenovationDetails.Add(renovationDetail);
        return entity.State == Microsoft.EntityFrameworkCore.EntityState.Added;
    }
}