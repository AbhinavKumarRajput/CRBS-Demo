using System.Net;
using System.Net.Mail;

namespace CRBS.Helper
{
    public class EmailHelper
    {
        public static bool Send(string to, string subject, string msg)
        {
            try
            {
                var fromEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL");
                var password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
                var host = Environment.GetEnvironmentVariable("SMTP_HOST");
                var port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT"));

                MailMessage message = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = msg,
                    IsBodyHtml = true
                };

                message.To.Add(to);

                SmtpClient smtpClient = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true
                };

                smtpClient.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("EMAIL ERROR: " + ex.Message);
                return false;
            }
        }
    }
}
