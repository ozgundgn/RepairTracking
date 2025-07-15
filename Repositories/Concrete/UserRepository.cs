using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairTracking.Data;
using RepairTracking.Data.Models;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.Repositories.Concrete;

public class UserRepository(AppDbContext context) : BaseContext(context), IUserRepository
{
    public async Task<User?> GetUserAsync(string username, string password)
    {
        var user = await Context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == username && u.Password == password && !u.Passive);
        return user;
    }
}