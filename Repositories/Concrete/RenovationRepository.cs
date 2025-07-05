using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RepairTracking.Data;
using RepairTracking.Data.Models;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.Repositories.Concrete;

public class RenovationRepository : BaseContext, IRenovationRepository
{
    public RenovationRepository(AppDbContext context) : base(context)
    {
    }

    public bool AddRenovation(Renovation renovation)
    {
        var entity = context.Renovations.Add(renovation);
        return entity.State == Microsoft.EntityFrameworkCore.EntityState.Added;
    }

    public bool UpdateRenovation(Renovation renovation)
    {
        var entity = context.Renovations.Update(renovation);
        return entity.State == EntityState.Modified;
    }

    public bool DeleteRenovation(int id)
    {
        var entity = context.Renovations.Find(id);
        if (entity == null)
            return false;
        var result = context.Renovations.Remove(entity);
        return result.State == Microsoft.EntityFrameworkCore.EntityState.Deleted;
    }

    public Renovation? GetRenovationById(int id)
    {
        var entity = context.Renovations
            .Include(r => r.Vehicle)
            .ThenInclude(v => v.Customer)
            .Include(r => r.RenovationDetails)
            .FirstOrDefault(r => r.Id == id);
        return entity;
    }

    public List<Renovation> GetRenovationsByVehcileIds(int[] ids)
    {
        var renovationList = context.Renovations.Where(x=> ids.Contains(x.VehicleId))
            .Include(r => r.Vehicle)
            .ThenInclude(v => v.Customer)
            .Include(r => r.RenovationDetails)
            .ToList();
        return renovationList;
    }
    public bool DeleteRenovationDetails(List<RenovationDetail> renovations)
    {
        context.RenovationDetails
            .RemoveRange(renovations);
        return context.SaveChanges() > 0;
    }

    public bool AddRenovationDetail(RenovationDetail renovationDetail)
    {
        var entity = context.RenovationDetails.Add(renovationDetail);
        return entity.State == Microsoft.EntityFrameworkCore.EntityState.Added;
    }
}