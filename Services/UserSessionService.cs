using RepairTracking.Data.Models;

namespace RepairTracking.Services;

public class UserSessionService
{
    public User? CurrentUser { get; private set; }

    public bool IsLoggedIn => CurrentUser != null;

    public void Login(User user)
    {
        CurrentUser = user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}