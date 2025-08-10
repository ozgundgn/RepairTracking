using System;
using System.IO;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace RepairTracking.Services;

public class MailKitSmptClient : IMessageSender
{
    public void SendEmailAsync(string body, string attachmentPath = "path/to/file.pdf")
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Repair Tracking", "ozgundgn0@gmail.com"));
        emailMessage.To.Add(new MailboxAddress("Recipient", "ozgundgn0@gmail.com"));
        emailMessage.Subject = "Email From Website";

        // var bodyBuilder = new BodyBuilder { TextBody = body };
        // emailMessage.Body = bodyBuilder.ToMessageBody();

        var bodyPart = new TextPart("plain")
        {
            Text = "Hello, this email contains an attachment."
        };

        // Create the attachment part
        var attachment = new MimePart("application", "pdf")
        {
            Content = new MimeContent(File.OpenRead(attachmentPath)),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = "file.pdf"
        };

        // Combine the body and the attachment into a multipart/mixed container
        var multipart = new Multipart("mixed");
        multipart.Add(bodyPart);
        multipart.Add(attachment);

        emailMessage.Body = multipart;
        
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
    void SendEmailAsync(string body,string a);
}