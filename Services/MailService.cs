using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace RepairTracking.Services;

public class MailService(string toMail, string? filePath = null) : IMailService
{
    public string ToMail { get; set; } = toMail;
    public string FilePath { get; set; } = filePath;


    public void SendMessage(string subject, string messageText)
    {
        string to = toMail; //To address
        string from = "ozenir@ozenir.com"; //From address
        MailMessage message = new MailMessage(from, to);

        string mailbody = "In this article you will learn how to send a email using Asp.Net & C#";
        message.Subject = "Sending Email Using Asp.Net & C#";
        message.Body = mailbody;
        message.BodyEncoding = Encoding.UTF8;
        message.IsBodyHtml = true;
        if (!string.IsNullOrWhiteSpace(FilePath))
            message.Attachments.Add(new Attachment(FilePath));

        try
        {
            const string emailfrom = "ozeniroto@yandex.com"; //Your yandex email adress
            var password = "ykdkcklnfwpevlwz"; //Your yandex app password
            using var smtpClient = new SmtpClient("smtp.yandex.com", 587);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(emailfrom, password);

            smtpClient.Send(emailfrom, ToMail, "Subject of mail", "Content of mail");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        // NetworkCredential basicCredential1 = new
        //     NetworkCredential("ozeniroto@yandex.com", "nicpgivuqvdjlfcd");
        // client.EnableSsl = true;
        // client.UseDefaultCredentials = false;
        // client.Credentials = basicCredential1;
        //
        // try
        // {
        //     client.Send(message);
        // }
        //
        // catch (Exception ex)
        // {
        //     throw ex;
        // }
        // finally
        // {
        //     ServicePointManager.ServerCertificateValidationCallback = null;
        // }
    }
}