using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairTracking.Data;
using RepairTracking.Data.Models;
using RepairTracking.Models;
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

    public async Task<bool?> UpdateUserPasswordAsync(int userId, string newPassword)
    {
        var user = await Context.Users.FindAsync(userId);
        if (user == null) return null;

        user.Password = newPassword;
        var response = Context.Users.Update(user);
        return response.State == EntityState.Modified;
    }

    public async Task<User?> GetUserByUsernameAsync(string userName)
    {
        return new User();
    }

    public List<UserInfo> GetActiveUsers()
    {
        var users = Context.Users.AsNoTracking().Where(x => !x.Passive);

        return users.Select(user => new UserInfo
        {
            GuidId = user.UserId,
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Username = user.UserName,
            PhoneNumber = user.Phone
        }).ToList();
    }

    public bool DeleteUser(int userId)
    {
        var result = Context.Users.Where(x => x.Id == userId).ExecuteUpdate(x => x.SetProperty(y => y.Passive, true));
        return result > 0;
    }

    public async Task<bool> AddUserAsync(User user)
    {
        var result = await Context.Users.AddAsync(user);
        return result.State == EntityState.Added;
    }

    public async Task<bool> UpdateUserAsync(int userId, string name, string surname, string username, string phone)
    {
        var result = await Context.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.Name, name)
                .SetProperty(y => y.Surname, surname)
                .SetProperty(y => y.UserName, username)
                .SetProperty(y => y.Phone, phone));
        return result > 0;
    }

    public async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }
}