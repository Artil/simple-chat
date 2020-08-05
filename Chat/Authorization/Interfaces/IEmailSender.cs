using ChatDbCore.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Interfaces
{
    public interface IEmailSender
    {
        Task ConfigureRegisterEmailAsync(User user);
        Task<bool> ConfigureChangePasswordEmailAsync(User user);
        Task ConfigureEmailAsync(User user, string title, string message);
        Task SendEmailAsync(string email, string subject, string message);
    }
}
