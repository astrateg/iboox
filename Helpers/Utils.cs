using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Web.Security;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace iBoox.Helpers
{
    public static class Utils
    {
        public static string HashResetParams(string username, string guid)
        {
            byte[] bytesofLink = Encoding.UTF8.GetBytes(username + guid);
            MD5 md5 = new MD5CryptoServiceProvider();
            string hashParams = BitConverter.ToString(md5.ComputeHash(bytesofLink));

            return hashParams;
        }

        // Настройки для отправки писем
        private const string SenderEmail = "alexejko@mail.ru";
        private const string SenderPswd = "Chess-master";

        public static void SendResetEmail(MembershipUser user, string hostUri)
        {
            //var smtp = new SmtpClient("smtp.mail.ru", 25);
            var smtp = new SmtpClient();

            var message = new MailMessage(SenderEmail, user.Email.ToLower());
            var sendCredential = new NetworkCredential(SenderEmail, SenderPswd);
            smtp.Credentials = sendCredential;
            message.SubjectEncoding = Encoding.UTF8;
            message.Subject = "Восстановление пароля iBoox";
            message.IsBodyHtml = true;
            string link = hostUri + 
                            "/Account/ResetPassword/?user=" + 
                            user.UserName + "&reset=" + 
                            HashResetParams(user.UserName, user.ProviderUserKey.ToString());
            message.Body = "<p>Здравствуйте!<br/> Для сброса своего старого пароля пeрeйдите по следующей ссылке: <a href='" + link + "'>" + link + "</a></p>";
            message.Body += "<p>Если Вы не запрашивали сброс пароля, то просто проигнорируйте это письмо.</p>";

            smtp.Send(message);
        }
    }
}