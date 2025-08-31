using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairTracking.Data;
using RepairTracking.Data.Models;
using RepairTracking.Repositories.Abstract;

namespace RepairTracking.Repositories.Concrete;

public class MailRepository(AppDbContext context) : BaseContext(context), IMailRepository
{
    public async Task<Mail?> GetMailTemplateAsync(string templateName)
    {
        var mail = await Context.Mails.FirstOrDefaultAsync(x=>x.Type == templateName);
        return mail;
    }

    public async Task<bool> SaveMailTemplateAsync(int id, string subject, string template)
    {
        var existingMail = await Context.Mails.FindAsync(id);
        if (existingMail == null)
        {
            return false;
        }

        existingMail.Subject = subject;
        existingMail.Template = template;

        Context.Mails.Update(existingMail);
        await Context.SaveChangesAsync();
        return true;
    }
}