using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using Serilog;

namespace RepairTracking.Services;

public class MailService(string toMail, string? filePath = null) : IMailService
{
    public string ToMail { get; set; } = toMail;
    public string FilePath { get; set; } = filePath;


    public void SendMessage(string subject, string messageText, string customerName)
    {
        string to = toMail; //To address
        string from = "ozeniroto@yandex.com"; //Your yandex email adress
        MailMessage message = new MailMessage(from, to);

        var mailbody = new StringBuilder(messageText);
        if (!string.IsNullOrWhiteSpace(messageText) && messageText.Contains("{MUSTERIADI}"))
        {
            mailbody = mailbody.Replace("{MUSTERIADI}", customerName);
        }

        message.Subject = subject;
        message.Body = GetHtmlBody(mailbody.ToString());
        message.BodyEncoding = Encoding.UTF8;
        message.IsBodyHtml = true;
        message.CC.Add("ozgundgn0@gmail.com");
        if (!string.IsNullOrWhiteSpace(FilePath))
        {
            message.Attachments.Add(new Attachment(FilePath));
        }

        try
        {
            var password = "ykdkcklnfwpevlwz"; //Your yandex app password
            using var smtpClient = new SmtpClient("smtp.yandex.com", 587);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(from, password);

            smtpClient.Send(message);
            Log.Logger.Information(
                $"Mail Gönderildi. {customerName} Mail template: {mailbody}");
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message);
        }
    }

    static string GetHtmlBody(string body)
    {
        // HTML content of the email body
        return $@"
            <html>
                <body>
                    {body}
                    <footer>
                        <p style='font-size: 12px; color: grey;'>Özenir Oto Bakım & Tamir</p>
                    </footer>
                </body>
            </html>";
    }
}