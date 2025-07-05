namespace RepairTracking.Models;

public class AppState
{
    private static AppState _instance;
    public static AppState Instance => _instance ??= new AppState();

    public UserInfo? LoggedInUser { get; private set; }

    public void SetUser(UserInfo? userInfo)
    {
        LoggedInUser = userInfo;
    }
    
    public void ClearUser()
    {
        LoggedInUser = null;
    }

    public bool IsAuthenticated => LoggedInUser != null;
}