using System.Threading;
using System.Threading.Tasks;

namespace RepairTracking.Repositories.Abstract;

public interface IBaseContext
{
    Task SaveChangesAsync(CancellationToken? cancellationToken=null);
}