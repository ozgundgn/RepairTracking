using System.Collections.Generic;
using System.Threading.Tasks;
using RepairTracking.Data.Models;
using RepairTracking.Models;

namespace RepairTracking.Repositories.Abstract;

public interface IUserRepository : IBaseContext
{
    Task<User?> GetUserAsync(string username, string password);
    Task<bool?> UpdateUserPasswordAsync(int userId, string newPassword);
    Task<User?> GetUserByUsernameAsync(string username);
    List<UserInfo> GetActiveUsers();
    bool DeleteUser(int userId); 
    Task<bool> AddUserAsync(User user);
    Task<bool> UpdateUserAsync(int userId, string name, string surname, string username, string phone,string email);
    Task SaveChangesAsync();
    
    Task UpdateUserCodeAsync(int userId, string code);
    Task ConfirmUserCodeAsync(int userId, string code);
    
    
}