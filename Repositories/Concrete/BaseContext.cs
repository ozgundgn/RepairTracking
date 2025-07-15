using RepairTracking.Data;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.Repositories.Concrete;

public class BaseContext : IBaseContext
{
    protected readonly AppDbContext Context;

    protected BaseContext(AppDbContext context)
    {
        Context = context;
    }
    
}