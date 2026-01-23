using System;
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
                var password  = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
                var host      = Environment.GetEnvironmentVariable("SMTP_HOST");
                var portStr   = Environment.GetEnvironmentVariable("SMTP_PORT");

                if (string.IsNullOrWhiteSpace(fromEmail) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(host) ||
                    string.IsNullOrWhiteSpace(portStr))
                {
                    Console.WriteLine("SMTP ENV VARIABLES NOT FOUND");
                    return false;
                }

                int port = int.Parse(portStr);

                MailMessage message = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = msg,
                    IsBodyHtml = true
                };

                message.To.Add(to);

                using var smtpClient = new SmtpClient(host, port)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail, password),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                smtpClient.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SMTP ERROR: " + ex.Message);
                return false;
            }
        }
    }
}
