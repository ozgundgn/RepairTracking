using System.Threading.Tasks;
using RepairTracking.Data.Models;

namespace RepairTracking.Repositories.Abstract;

public interface IMailRepository: IBaseContext
{
    Task<Mail?> GetMailTemplateAsync(string templateName);
    Task<bool> SaveMailTemplateAsync(int id, string subject, string template);
}