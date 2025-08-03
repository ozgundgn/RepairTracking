namespace RepairTracking.Services;

public class NotificationFactory(INotifyService notifyService)
{
    public void SendMessage(string subject,string message)
    {
        notifyService.SendMessage(subject,message);
    }
}