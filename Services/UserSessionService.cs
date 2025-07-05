using RepairTracking.Data.Models;
using RepairTracking.Models;

namespace RepairTracking.Services;

public class UserSessionService
{
    public UserInfo? CurrentUser { get; private set; }

    public bool IsLoggedIn => CurrentUser != null;

    public void Login(UserInfo user)
    {
        CurrentUser = user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}