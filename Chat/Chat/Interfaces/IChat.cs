using ChatCore.Enums;
using ChatCore.Models;
using ChatDbCore.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Interfaces
{
    public interface IChat
    {
        Task<IEnumerable<ChatListModel>> GetChats();
        bool IsUsersInChat(User user, string currentUser);
        Task<bool> IsChatExist(User user, User companion, MessageModel message);
        void CreateNewChat(User user, User companion, MessageModel message);
        Task<string> CreateGroup(GroupModel groupModel);
        IEnumerable<GroupModel> SearchGroupsRequest(string query);
        Task<GroupModel> GetGroup(string groupId);
        Task UpdateGroup(GroupModel group);
    }
}
