namespace RepairTracking.Services;

public class NotificationFactory(INotifyService notifyService)
{
    public void SendMessage(string subject, string message, string customerName)
    {
        notifyService.SendMessage(subject, message, customerName);
    }
}