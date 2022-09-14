using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace ddPoliglotV6.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            string emailFrom = "auth@ddpoliglot.com";
            var mm = new MailMessage(emailFrom, email);
            mm.Subject = subject;
            mm.Body = message;
            mm.IsBodyHtml = true;
            var smtp = new SmtpClient();
            smtp.Host = "mail.ddpoliglot.com";
            smtp.EnableSsl = false;
            NetworkCredential NetworkCred = new NetworkCredential("auth@ddpoliglot.com", "b9!!31Lasdf1zsdfsdfgg");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            // smtp.Send(mm);
            return smtp.SendMailAsync(mm);
        }

        //public Task SendEmailAsync(string email, string subject, string message)
        //{
        //    string emailFrom = "ddpoliglot@gmail.com";
        //    var mm = new MailMessage(emailFrom, email);
        //    mm.Subject = subject;
        //    mm.Body = message;
        //    mm.IsBodyHtml = true;
        //    var smtp = new SmtpClient();
        //    smtp.Host = "smtp.gmail.com";
        //    smtp.EnableSsl = true;
        //    NetworkCredential NetworkCred = new NetworkCredential(emailFrom, "ddPoliglot_b9!!31Lasdf1z");
        //    smtp.UseDefaultCredentials = false;
        //    smtp.Credentials = NetworkCred;
        //    smtp.Port = 587;
        //    // smtp.Send(mm);
        //    return smtp.SendMailAsync(mm);
        //}


        //public Task Execute(string subject, string message, string email)
        //{

        //    //var client = new SendGridClient(apiKey);
        //    //var msg = new SendGridMessage()
        //    //{
        //    //    From = new EmailAddress("Joe@contoso.com", Options.SendGridUser),
        //    //    Subject = subject,
        //    //    PlainTextContent = message,
        //    //    HtmlContent = message
        //    //};
        //    //msg.AddTo(new EmailAddress(email));

        //    //// Disable click tracking.
        //    //// See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        //    //msg.SetClickTracking(false, false);

        //    //return client.SendEmailAsync(msg);

        //}
    }

}
