using ChatCore;
using ChatCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatDesktop.Interfaces
{
    public interface IAccount
    {
        Task<dynamic> Login(string data);
        Task<dynamic> Register(string data);
        Task<bool> ResetPassword(string userName);
        Task SwapKeys(string publicKey);
    }
}
