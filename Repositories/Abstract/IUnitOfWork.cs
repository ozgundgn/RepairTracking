using System.Threading;
using System.Threading.Tasks;

namespace RepairTracking.Repositories.Abstract;

public interface IUnitOfWork
{
    public IVehicleRepository VehiclesRepository { get; }
    public ICustomerRepository CustomersRepository { get; }
    public IRenovationRepository RenovationsRepository { get; }
    public ICustomersVehiclesRepository CustomersVehiclesRepository { get; }
    public IUserRepository UsersRepository { get; }
    public IMailRepository MailRepository { get; }
    
    Task SaveChangesAsync(CancellationToken? cancellationToken = null);
}