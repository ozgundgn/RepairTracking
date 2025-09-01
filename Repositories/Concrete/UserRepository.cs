using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

    public async Task<User?> GetUserAsync(Expression<Func<User, bool>> predicate)
    {
        var user = await Context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == "admin" && u.Password == "admin" && !u.Passive);
        return user;
    }


    public async Task<bool?> UpdateUserPasswordAsync(int userId, string newPassword)
    {
        var user = await Context.Users.FindAsync(userId);
        if (user == null) return null;

        user.Password = newPassword;
        var response = Context.Users.Update(user);
        var result = response.State == EntityState.Modified;
        await SaveChangesAsync();
        return result;
    }

    public async Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await Context.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserName == usernameOrEmail || x.Email == usernameOrEmail && !x.Passive);
    }

    public async Task<User?> GetUserByPhoneAndUsernameAsync(string phone, string username, int? userdId = null)
    {
        var queryable = Context.Users.AsNoTracking()
            .Where(x => (x.Phone == phone || x.UserName == username) && !x.Passive);
        if (userdId.HasValue)
            queryable = queryable.Where(x => x.Id != userdId.Value);
        var user = await queryable.FirstOrDefaultAsync();
        return user;
    }
    
    public async Task<User?> GetUserByEmailAndUsernameAsync(string email, string username, int? userdId = null)
    {
        var queryable = Context.Users.AsNoTracking()
            .Where(x => (x.Email == email || x.UserName == username) && !x.Passive);
        if (userdId.HasValue)
            queryable = queryable.Where(x => x.Id != userdId.Value);
        var user = await queryable.FirstOrDefaultAsync();
        return user;
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
            PhoneNumber = user.Phone,
            Email = user.Email,
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

    public async Task<bool> UpdateUserAsync(int userId, string name, string surname, string username, string phone,
        string email)
    {
        var result = await Context.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.Name, name)
                .SetProperty(y => y.Surname, surname)
                .SetProperty(y => y.UserName, username)
                .SetProperty(y => y.Email, email)
                .SetProperty(y => y.Phone, phone));
        return result > 0;
    }

    public async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }

    public async Task UpdateUserCodeAsync(int userId, string code)
    {
        await Context.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.Code, code)
                .SetProperty(y => y.Confirmed, false));
    }

    public async Task<bool> ConfirmUserCodeAsync(int userId, string code)
    {
        var user = await Context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId && x.Code == code);

        if (user != null)
        {
            user.Confirmed = true;
            Context.Users.Update(user);
            await Context.SaveChangesAsync();
        }

        return user != null && user.Confirmed == true;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await Context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId && !x.Passive);
    }
}