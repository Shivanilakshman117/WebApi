using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;

namespace WebApi.Utilities
{
    public class ComposeMail
    {
        public static MailMessage CompleteRegistration(string emailID, string code, out SmtpClient messenger)
        {
            var fromEmail = new MailAddress("techxems@gmail.com", "EMS Portal");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Psiog@123";
            string subject = "";
            string body = "";
            subject = "Welcome Aboard!";
            body = "Dear Associate,<br/><br/>\nWe are delighted to welcome you on board with us! Please complete the registration for our Employee Management System Portal by following the link given below. The link will expire in 1 day." +
                    "<br/><br/><a href=";

            messenger = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            var mail = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            return mail;
        }
        public static MailMessage SendResetLink(string emailID, out SmtpClient messenger)
        {
            var fromEmail = new MailAddress("techxems@gmail.com", "EMS Portal");
            var toEmail = new MailAddress("techxems@gmail.com");
            var fromEmailPassword = "Psiog@123";
            string subject = "";
            string body = "";
            subject = "Psiog EMS Portal: Request For Reset Password";
            body = "Dear Associate,<br/><br/>\nWe heard that you lost your EMS Portal password. You can use the following link to reset your password:" +
                    "<br/><br/><a href=";

            messenger = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            var mail = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            return mail;
        }
    }
}