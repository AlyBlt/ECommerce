using ECommerce.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmail(string email, string resetLink)
        {
            var smtpSettings = _configuration.GetSection("Email").GetChildren().ToList();
            bool emailSent = false;

            foreach (var smtpSetting in smtpSettings)
            {
                try
                {
                    var smtpServer = smtpSetting.GetValue<string>("SmtpServer");
                    var smtpPort = smtpSetting.GetValue<int>("SmtpPort");
                    var username = smtpSetting.GetValue<string>("Username");
                    var password = smtpSetting.GetValue<string>("Password");
                    var fromAddress = smtpSetting.GetValue<string>("FromAddress");

                    var smtpClient = new SmtpClient(smtpServer)
                    {
                        Port = smtpPort,
                        Credentials = new NetworkCredential(username, password),
                        EnableSsl = true
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromAddress),
                        Subject = "Password Reset",
                        Body = $"Please reset your password by clicking <a href=\"{resetLink}\">here</a>.",
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    // E-posta gönderimi
                    await smtpClient.SendMailAsync(mailMessage);
                    emailSent = true;
                    break; // E-posta başarılı gönderildiyse, döngüyü kır
                }
                catch (Exception ex)
                {
                    // Hata durumunda, diğer sağlayıcıya geçmeye devam et
                    Console.WriteLine($"Error sending email via {smtpSetting.Key}: {ex.Message}");
                }
            }

            if (!emailSent)
            {
                throw new Exception("Failed to send email via all available providers.");
            }
        }
    }
}
