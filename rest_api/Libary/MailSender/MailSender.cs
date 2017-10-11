using System;
using System.Net.Mail;
using rest_api.ModelViews;
namespace rest_api.Libary.MailSender
{
    public static class MailSender
    {
        public static bool Send(_Contact mail)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("bildirim@menkule.com.tr");
                mailMessage.To.Add("iletisim@menkule.com.tr");
                mailMessage.Subject = DateTime.Now.ToString() + " " + mail.name + " İletişim Formu";
                mailMessage.Body = "<b>" + mail.name + "</b>: " + mail.message + "<br/> <b>IP Adresi:</b>: ";
                mailMessage.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = true;
                smtp.Host = "smtp.yandex.com.tr";
                smtp.Credentials = new System.Net.NetworkCredential("bildirim@menkule.com.tr", "mercksap651");
                smtp.Send(mailMessage);
            }
            catch (SmtpException ex)
            {
                return false;
            }
            return true;
           
        }
    }
}