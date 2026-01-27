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
                var fromEmail = Environment.GetEnvironmentVariable("BREVO_EMAIL");
                var password  = Environment.GetEnvironmentVariable("BREVO_PASS");
                var host      = Environment.GetEnvironmentVariable("BREVO_HOST");
                var portStr   = Environment.GetEnvironmentVariable("BREVO_PORT");

                if (string.IsNullOrWhiteSpace(fromEmail) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(host) ||
                    string.IsNullOrWhiteSpace(portStr))
                {
                    Console.WriteLine("BREVO SMTP ENV VARIABLES NOT FOUND");
                    return false;
                }

                int port = int.Parse(portStr);

                using MailMessage message = new MailMessage
                {
                    From = new MailAddress(fromEmail, "CRBS Team"),
                    Subject = subject,
                    Body = msg,
                    IsBodyHtml = true
                };

                message.To.Add(to);

                using SmtpClient smtpClient = new SmtpClient(host, port)
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
                Console.WriteLine("BREVO SMTP ERROR: " + ex.Message);
                return false;
            }
        }
    }
}
