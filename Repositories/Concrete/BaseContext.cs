using System.Threading;
using System.Threading.Tasks;
using RepairTracking.Data;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.Repositories.Concrete;

public class BaseContext : IBaseContext
{
    protected AppDbContext context;

    public BaseContext(AppDbContext context)
    {
        this.context = context;
    }

    public async Task SaveChangesAsync(CancellationToken? cancellationToken)
    {
        await context.SaveChangesAsync();
    }
}