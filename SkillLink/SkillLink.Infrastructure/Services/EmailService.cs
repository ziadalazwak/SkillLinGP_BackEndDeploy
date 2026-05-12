using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace SkillLink.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendOtpEmail(string email, string otp)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SkillLink", "ziadziad@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "OTP Code";
            message.Body = new TextPart("plain") { Text = $"Your OTP is: {otp}" };

            using var client = new SmtpClient();
            client.AuthenticationMechanisms.Remove("XOAUTH2"); // Gmail app password

            // DEV-SAFE SSL bypass for revocation error
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("ziadziadnaser@gmail.com", "ycky dmmm gjxy wwsu");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
