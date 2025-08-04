using System;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace RepairTracking.Services;

public class MailKitSmptClient : IMessageSender
{
    public void SendEmailAsync(string body)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Repair Tracking", "ozgundgn0@gmail.com"));
        emailMessage.To.Add(new MailboxAddress("Recipient", "ozgundgn0@gmail.com"));
        emailMessage.Subject = "Email From Website";

        var bodyBuilder = new BodyBuilder { TextBody = body };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput())))
        {
            client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            client.Authenticate("ozgundgn0@gmail.com", "qvoc topa shnx oddj");
            client.Send(emailMessage);
            client.Disconnect(true);
        }
    }
}

public interface IMessageSender
{
    void SendEmailAsync(string body);
}