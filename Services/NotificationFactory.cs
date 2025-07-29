namespace RepairTracking.Services;

public class NotificationFactory(INotifyService notifyService)
{
    public void SendMessage(string message)
    {
        notifyService.SendMessage(message);
    }
}