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
                // Hardcoded email + SMTP key
                string fromEmail = "manusingh64431@gmail.com";
                string password = "xsmtpsib-a06dc43a4e5f22d973d5fb5e1e11050a42917c5be3185a6f6e9e7413f190ea6f-klFbeRnCKfy5OYBc";
                string host = "smtp-relay.brevo.com";
                int port = 587;

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
                Console.WriteLine("Email sent to " + to);
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
