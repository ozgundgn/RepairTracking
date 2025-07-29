using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RepairTracking.Services;

public class MailService : IMailService
{
    public void SendMessage(string messageText)
    {
        string to = "ozgundgn0@gmail.com"; //To address
        string from = "ozenir@ozenir.com"; //From address
        MailMessage message = new MailMessage(from, to);  
  
        string mailbody = "In this article you will learn how to send a email using Asp.Net & C#";  
        message.Subject = "Sending Email Using Asp.Net & C#";  
        message.Body = mailbody;  
        message.BodyEncoding = Encoding.UTF8;  
        message.IsBodyHtml = true;  
        SmtpClient client = new SmtpClient("mail.ozenir.com", 587); //Gmail smtp    
        System.Net.NetworkCredential basicCredential1 = new  
            System.Net.NetworkCredential("ozenir@ozenir.com", "ozenir.1594");  
        client.EnableSsl = true;  
        client.UseDefaultCredentials = false;  
        client.Credentials = basicCredential1;  
  
        try
        {
            client.Send(message);
        }

        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            ServicePointManager.ServerCertificateValidationCallback = null;
        }
    }
}