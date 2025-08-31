using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RepairTracking.Data.Models;
using RepairTracking.Models;

namespace RepairTracking.Repositories.Abstract;

public interface IUserRepository : IBaseContext
{
    Task<User?> GetUserAsync(string username, string password);
    Task<User?> GetUserAsync(Expression<Func<User, bool>> predicate);

    Task<User?> GetUserByIdAsync(int userId);
    Task<bool?> UpdateUserPasswordAsync(int userId, string newPassword);
    Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
    Task<User?> GetUserByPhoneAndUsernameAsync(string phone, string username, int? userdId = null);
    List<UserInfo> GetActiveUsers();
    bool DeleteUser(int userId);
    Task<bool> AddUserAsync(User user);
    Task<bool> UpdateUserAsync(int userId, string name, string surname, string username, string phone, string email);
    Task SaveChangesAsync();
    Task UpdateUserCodeAsync(int userId, string code);
    Task<bool> ConfirmUserCodeAsync(int userId, string code);
}