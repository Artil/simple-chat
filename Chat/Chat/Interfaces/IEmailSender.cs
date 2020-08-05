using ChatDbCore.Account;
using System.Threading.Tasks;

namespace ChatServer.Interfaces
{
    public interface IEmailSender
    {
        Task ConfigureEmailAsync(User user);
        Task SendEmailAsync(string email, string subject, string message);
    }
}
