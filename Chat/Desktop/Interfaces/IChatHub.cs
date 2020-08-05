using ChatCore.Enums;
using ChatCore.Models;
using ChatDbCore.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatDesktop.Interfaces
{
    public interface IChatHub
    {
        event Action ConnectionStarted;
        event Action<MessageModel> NewCompanionMessage;
        event Action<ChatListModel> NewChat;
        event Action<MessageModel> MessageDeleted;
        event Action<MessageModel> MessageUpdated;
        void ChatHubManager();
        Task Connect();
        Task<UserModel> GetUser(string userName = null);
        Task<IEnumerable<UserListModel>> GetRequestUserList(string query, SearchByEnum searchBy);
        Task<IEnumerable<GroupModel>> GetRequestGroupList(string query);
        Task<IEnumerable<UserListModel>> GetContactList();
        Task<GroupModel> GetGroup(string groupId);
        Task<bool> IsUserOnlineCheck(string userName);
        Task<IEnumerable<MessageModel>> GetMessages(string chatId, int count = 0);
        Task<MessageModel> SendMessage(MessageModel message);
        Task<IEnumerable<ChatListModel>> GetUserChats();
        Task DeleteMessage(MessageModel message, bool isForMe = false);
        Task<MessageModel> UpdateMessage(MessageModel message);
        Task<MessageModel> GetLastMessage(string messageId);
        Task<MessageModel> ForwardMessage(string chatId, string messageId);
        Task UpdateUserInfo(UserModel userInfo);
        Task ReadMessages(string chatId);
        Task<string> CreateGroup(GroupModel groupModel);
        Task UpdateGroup(GroupModel group);
        Task<bool> ChangePassword(ChangePasswordViewModel changePasswordVM);
    }
}
