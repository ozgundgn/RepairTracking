using System;
using System.Collections.Generic;
using RepairTracking.Data.Models;

namespace RepairTracking.Repositories.Abstract;

public interface IRenovationRepository : IBaseContext
{
    void AddRenovation(Renovation renovation);
    bool UpdateRenovation(Renovation renovation);
    bool DeleteRenovation(int id);
    bool DeleteRenovationDeliveryDate(int id);

    bool PassiveRenovation(int vehicleId);
    Renovation? GetRenovationById(int id);
    bool DeleteRenovationDetails(List<RenovationDetail> renovationDetails);
    bool AddRenovationDetail(RenovationDetail renovationDetail);
    List<Renovation> GetRenovationsByVehcileIds(int[]? ids);
    bool UpdateRenovationDeliveryDate(int id, DateTime datetime);
    Renovation? GetLastRenovation(int vehicleId);
    bool UpdateRenovationReportPath(int id, string path);
    void SaveChanges();
}