using System;
using System.Collections.Generic;
using RepairTracking.Data.Models;

namespace RepairTracking.Repositories.Abstract;

public interface IRenovationRepository : IBaseContext
{
    bool AddRenovation(Renovation renovation);
    bool UpdateRenovation(Renovation renovation);
    bool DeleteRenovation(int id);
    bool PassiveRenovation(int vehicleId);
    Renovation? GetRenovationById(int id);
    bool DeleteRenovationDetails(List<RenovationDetail> renovations);
    bool AddRenovationDetail(RenovationDetail renovationDetail);
    List<Renovation> GetRenovationsByVehcileIds(int[]? ids);
    bool UpdateRenovationDeliveryDate(int id,DateTime datetime);
}