using ChatServer.Interfaces;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Web;
using Microsoft.AspNetCore.Http;
using System;
using ChatDbCore.Account;

namespace ChatServer.Services
{
    class EmailService : IEmailSender
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly HttpRequest _httpRequest;

        public EmailService(UserManager<User> userManager, IConfiguration config, IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _config = config;
            _httpRequest = httpContext.HttpContext.Request;
        }

        public async Task ConfigureEmailAsync(User user)
        {
            var code = HttpUtility.UrlEncode( await _userManager.GenerateEmailConfirmationTokenAsync(user));
            try
            {
                var callbackUrl = $"{_httpRequest.Scheme}://{_httpRequest.Host}/Email/ConfirmEmail?userId={user.Id}&code={code}";
                await SendEmailAsync(user.Email, "Confirm your account", $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");
            } catch (Exception ex)
            {
            }
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", _config.GetSection("EmailConnection").GetSection("Email").Value));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync(_config.GetSection("EmailConnection").GetSection("Email").Value, _config.GetSection("EmailConnection").GetSection("Password").Value);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
