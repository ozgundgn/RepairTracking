namespace RepairTracking.Services;

public interface INotifyService
{
   public void SendMessage(string subject,string message,string customerName); 
}