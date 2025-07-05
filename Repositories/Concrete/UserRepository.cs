using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairTracking.Data;
using RepairTracking.Data.Models;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.Repositories.Concrete;

public class UserRepository : BaseContext, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetUserAsync(string username, string password)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.UserName == username && u.Password == password && !u.Passive);
        return user;
    }
}