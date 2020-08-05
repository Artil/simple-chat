using ChatCore.Enums;
using ChatCore.Models;
using ChatDbCore;
using ChatDbCore.Account;
using ChatDbCore.ChatModels;
using ChatServer.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer.Services
{
    public class UserService : UtilityService, IUser
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly IChat _chat;
        public UserService(Context context, UserManager<User> userManager, IChat chat) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _chat = chat;
        }

        public IEnumerable<UserListModel> SearchUsersRequest(string query, SearchByEnum searchBy)
        {
            var list = new List<User>();
            switch (searchBy)
            {
                case SearchByEnum.Name:
                    list = _userManager.Users
                        .Where(x => x.NormalizedUserName.Contains(query.ToUpper()) && x.UserName != Thread.CurrentPrincipal.Identity.Name).ToList();
                    break;

                case SearchByEnum.Email:
                    list = _userManager.Users
                        .Where(x => x.NormalizedEmail.Contains(query.ToUpper()) && x.UserName != Thread.CurrentPrincipal.Identity.Name).ToList();
                    break;

                case SearchByEnum.Phone:
                    list = _userManager.Users
                        .Where(x => x.PhoneNumber.Contains(query) && x.UserName != Thread.CurrentPrincipal.Identity.Name).ToList();
                    break;

                default:
                    return null;
            }

            list = list.Where(x => _chat.IsUsersInChat(x, Thread.CurrentPrincipal.Identity.Name)).ToList();

            return list.Select(z => new UserListModel() { Name = z.UserName, Email = z.Email, PhoneNumber = z.PhoneNumber });
        }

        public IEnumerable<UserListModel> GetContacts()
        {
            var userName = Thread.CurrentPrincipal.Identity.Name;

            return _context.Users.Where(x =>
                _context.ChatsForTwo.Any(y =>
                (String.Equals(x.UserName, y.FirstUser.UserName) && String.Equals(y.SecondUser.UserName, userName))
                || (String.Equals(y.FirstUser.UserName, userName) && String.Equals(x.UserName, y.SecondUser.UserName))))
                .Select(z => new UserListModel() { Name = z.UserName, Email = z.Email, PhoneNumber = z.PhoneNumber });
        }

        public async Task<UserModel> GetUserInfo(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userInfo = _context.UsersAccountInfo.FirstOrDefault(x => String.Equals(x.UserId, user.Id));
            var userModel = new UserModel();

            if(!ReferenceEquals(user, null))
            {
                SetValues(userModel, user);
                userModel.Name = user.UserName;
                userModel.Id = user.Id;
            }

            if (!ReferenceEquals(userInfo, null))
            {
                SetValues(userModel, userInfo);
                userModel.PhotoPath = userInfo.FileDb?.FilePath;
            }

            return userModel;
        }

        public async Task UpdateUserInfo(UserModel userModel)
        {
            var user = await _userManager.FindByNameAsync(userModel.Name);
            var userInfo = _context.UsersAccountInfo.FirstOrDefault(x => String.Equals(x.UserId, user.Id));

            SetValues(userInfo, userModel);

            if (!String.IsNullOrEmpty(userModel.PhotoPath))
            {
                if (userInfo.FileDb is null)
                {
                    userInfo.FileDb = new FileDb();
                    userInfo.FileDb.IsImage = true;
                }
                userInfo.FileDb.FilePath = userModel.PhotoPath;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.userName);
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }
}
