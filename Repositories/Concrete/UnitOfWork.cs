using System.Threading;
using System.Threading.Tasks;
using RepairTracking.Data;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.Repositories.Concrete;

public class UnitOfWork(
    AppDbContext context,
    IVehicleRepository vehiclesRepository,
    ICustomerRepository customersRepository,
    IRenovationRepository renovationsRepository,
    ICustomersVehiclesRepository customersVehiclesRepository,
    IUserRepository usersRepository,IMailRepository mailRepository)
    :BaseContext(context), IUnitOfWork
{
    public IVehicleRepository VehiclesRepository { get; } = vehiclesRepository;
    public ICustomerRepository CustomersRepository { get; } = customersRepository;
    public IRenovationRepository RenovationsRepository { get; } = renovationsRepository;
    public ICustomersVehiclesRepository CustomersVehiclesRepository { get; } = customersVehiclesRepository;
    public IUserRepository UsersRepository { get; } = usersRepository;
    public IMailRepository MailRepository { get; } = mailRepository;
    public async Task SaveChangesAsync(CancellationToken? cancellationToken = null)
    {
        await Context.SaveChangesAsync();
    }
}