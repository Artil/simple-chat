using ChatCore.Enums;
using ChatCore.Models;
using ChatDbCore.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Interfaces
{
    public interface IUser
    {
        IEnumerable<UserListModel> SearchUsersRequest(string query, SearchByEnum searchBy);
        IEnumerable<UserListModel> GetContacts();
        Task<UserModel> GetUserInfo(string userName);
        Task UpdateUserInfo(UserModel userInfo);
        Task<bool> ChangePassword(ChangePasswordViewModel model);
    }
}
