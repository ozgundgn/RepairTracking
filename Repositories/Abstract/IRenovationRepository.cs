using System.Collections.Generic;
using RepairTracking.Data.Models;

namespace RepairTracking.Repositories.Abstract;

public interface IRenovationRepository : IBaseContext
{
    bool AddRenovation(Renovation renovation);
    bool UpdateRenovation(Renovation renovation);
    bool DeleteRenovation(int id);
    Renovation? GetRenovationById(int id);
    bool DeleteRenovationDetails(List<RenovationDetail> renovations);
    bool AddRenovationDetail(RenovationDetail renovationDetail);
    List<Renovation> GetRenovationsByVehcileIds(int[]? ids);
}