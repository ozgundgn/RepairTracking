namespace RepairTracking.Services;

public interface IMailService : INotifyService
{
    public string ToMail { get; set; }
}