namespace RepairTracking.Models;

public class AppState
{
    private static AppState _instance;
    public static AppState Instance => _instance ??= new AppState();

    public User? LoggedInUser { get; private set; }

    public void SetUser(User user)
    {
        LoggedInUser = user;
    }

    public bool IsAuthenticated => LoggedInUser != null;
}