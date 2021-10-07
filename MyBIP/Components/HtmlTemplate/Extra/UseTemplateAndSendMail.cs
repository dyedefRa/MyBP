using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MyBIP.Components.HtmlTemplate.Extra
{
    public class UseTemplateAndSendMail
    {
        private readonly IWebHostEnvironment _env;

        public UseTemplateAndSendMail(IWebHostEnvironment env)
        {
            _env = env;
        }

        public bool SendCreateUserTemplateEmail()
        {
            string message = GetCreateUserTemplate();
            string tomail = "bkztrk@gmail.com";
            var toMailAddresses = tomail
                            .Split(';')
                            .Where(email => !string.IsNullOrEmpty(email.Trim()))
                            .Select(email => new MailAddress(email))
                                .ToArray();
                              
            return SendEmail("from", "displayname", "subject", message, null, null, null, toMailAddresses);
        }

        /// <summary>
        /// BURASI ONEMLI
        /// </summary>
        /// <returns></returns>
        private string GetCreateUserTemplate()
        {
            var emailTemplatePath = Path.Combine(_env.ContentRootPath, "Components/HtmlTemplate/Required/CreateUserEmailTemplate.html");

            //Burası Message kısmı. Doldurduk ve Template Çektik.
            var user = new { Firstname = "Ali", Lastname = "Veli", Email = "Mailinz@gmailçcom", Password = "1245as", PortalName = "Bayi İletişim Platformu", brandUrl = "https://localhost:44367/", brandDomain = "local.boschevaletleri.com" };

            var emailTemplate = File.ReadAllText(emailTemplatePath)
                   .Replace("@userFullName", user.Firstname + " " + user.Lastname)
                   .Replace("@email", user.Email)
                   .Replace("@password", user.Password)
                   .Replace("@portalName", user.PortalName)
                   .Replace("@portalLongName", "Bayi İletişim Portalı")
                   .Replace("@brandUrl", user.brandUrl)
                   .Replace("@brandDomain", user.brandDomain);
            return emailTemplate;
        }

        private bool SendEmail(string from, string displayName, string subject, string message, MailAddress[] ccAddresses, MailAddress[] bccAddresses, Attachment[] attachments, params MailAddress[] emailAddress)
        {
            try
            {
                var userName = "californiahacker_@hotmail.com";
                var password = "29723210084a";
                var smtpHost = "smtp.live.com";
                //from = from ?? ConfigurationManager.AppSettings["MailServerUsername"];
                var client = new SmtpClient(smtpHost, 587)
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(userName, password)
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(userName, displayName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                foreach (var email in emailAddress)
                {
                    mailMessage.To.Add(email);
                }
                if (ccAddresses != null)
                {
                    foreach (var mailAddress in ccAddresses)
                    {
                        mailMessage.CC.Add(mailAddress);
                    }
                }
                if (bccAddresses != null)
                {
                    foreach (var mailAddress in bccAddresses)
                    {
                        mailMessage.Bcc.Add(mailAddress);
                    }
                }
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        mailMessage.Attachments.Add(attachment);
                    }
                }
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.Error(new { subject, emailAddress }, ex);
            }
            return false;
        }
    }
}
