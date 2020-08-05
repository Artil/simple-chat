using Authorization.Interfaces;
using ChatDbCore.Account;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Authorization.Services
{
    public class EmailService : IEmailSender
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

        public async Task ConfigureRegisterEmailAsync(User user)
        {
            var code = HttpUtility.UrlEncode(await _userManager.GenerateEmailConfirmationTokenAsync(user));
            try
            {
                var callbackUrl = $"{_httpRequest.Scheme}://{_httpRequest.Host}/ConfirmEmail?userId={user.Id}&code={code}";
                await SendEmailAsync(user.Email, "Confirm your account", $"Confirm registration: <a href='{callbackUrl}'>link</a>");
            }
            catch { }
        }

        public async Task<bool> ConfigureChangePasswordEmailAsync(User user)
        {
            var code = HttpUtility.UrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user));
            try
            {
                var callbackUrl = $"{_httpRequest.Scheme}://{_httpRequest.Host}/ChangePassword?userId={user.Id}&code={code}";
                await SendEmailAsync(user.Email, "Change password", $"Confirm password change: <a href='{callbackUrl}'>link</a>");
                return true;
            }
            catch { return false; }
        }

        public async Task ConfigureEmailAsync(User user, string title, string message)
        {
            try
            {
                await SendEmailAsync(user.Email, title, message);
            }
            catch { }
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
