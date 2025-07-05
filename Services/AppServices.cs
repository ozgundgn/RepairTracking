namespace RepairTracking.Services;

public class AppServices
{
    public static UserSessionService UserSessionService { get; } = new();
    public static INavigationService NavigationService { get; set; }
}