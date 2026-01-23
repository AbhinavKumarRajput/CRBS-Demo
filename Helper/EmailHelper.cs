using System.Net;
using System.Net.Mail;

namespace CRBS.Helper
{
    public class EmailHelper
    {
        public static bool Send(string to, string subject, string msg)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            message.From = new MailAddress("manusingh64431@gmail.com");
            message.To.Add(to);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = msg;

            smtpClient.Port = 587;
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("manusingh64431@gmail.com", "dhkw sjae abaq ypis");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                smtpClient.Send(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
