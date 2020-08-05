using ChatCore.Enums;
using ChatCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChatDbCore.Account;
using ChatDbCore;
using ChatDbCore.ChatModels;
using ChatServer.Interfaces;
using System.Threading;
using System.Security.Principal;

namespace ChatServer.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly UserManager<User> _userManager;
        private readonly Context _context;
        private readonly IChat _chat;
        private readonly IMessage _message;
        private readonly IUser _user;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly static Dictionary<string, string> ConnectedUsers = new Dictionary<string, string>();

        public ChatHub(UserManager<User> userManager, Context context, IChat chat, IMessage message, IUser user)
        {
            _userManager = userManager;
            _context = context;
            _chat = chat;
            _message = message;
            _user = user;
        }

        public override Task OnConnectedAsync()
        {
            ConnectedUsers.Add(Context.User.Identity.Name, Context.ConnectionId);
            logger.Debug($"User {Context.User.Identity.Name} connected by id - {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectedUsers.Remove(Context.User.Identity.Name);
            logger.Debug($"User {Context.User.Identity.Name} log out. Connection id - {Context.ConnectionId} is closed");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task<UserModel> GetUser(string userName = null)
        {
            logger.Debug($"User {Context.User.Identity.Name} trying get info - {userName ?? Context.User.Identity.Name}");
            if (String.IsNullOrEmpty(userName))
                return await _user.GetUserInfo(Context.User.Identity.Name);
            else
                return await _user.GetUserInfo(userName);
        }

        public IEnumerable<UserListModel> SearchUsersRequest(string query, SearchByEnum searchBy)
        {
            Thread.CurrentPrincipal = Context.User;
            logger.Debug($"User {Context.User.Identity.Name} search by {searchBy} with param {query}");
            return _user.SearchUsersRequest(query, searchBy);
        }

        public IEnumerable<GroupModel> SearchGroupsRequest(string query)
        {
            logger.Debug($"User {Context.User.Identity.Name} search group with param {query}");
            return _chat.SearchGroupsRequest(query);
        }

        public IEnumerable<UserListModel> GetContacts()
        {
            Thread.CurrentPrincipal = Context.User;
            logger.Debug($"User {Context.User.Identity.Name} trying to get contacts");
            return _user.GetContacts();
        }

        public bool IsUserOnlineCheck(string userName)
        {
            return ConnectedUsers.Any(x => String.Equals(x.Key, userName));
        }

        public IEnumerable<MessageModel> GetMessages(string chatId, int count = 0)
        {
            Thread.CurrentPrincipal = Context.User;
            logger.Debug($"User {Context.User.Identity.Name} get messages from {chatId}");
            return _message.GetMessagesByChatId(chatId, count);
        }

        public async Task<IEnumerable<ChatListModel>> GetChats()
        {
            Thread.CurrentPrincipal = Context.User;
            logger.Debug($"User {Context.User.Identity.Name} get chats");
            return await _chat.GetChats();
        }

        public async Task<MessageModel> SendMessage(MessageModel message)
        {
            Thread.CurrentPrincipal = Context.User;
            logger.Debug($"User {Context.User.Identity.Name} send message to {message.ToChatName}");
            return await _message.SendMessage(message);
        }       

        public async Task DeleteMessage(MessageModel message, bool isForMe = false)
        {
            Thread.CurrentPrincipal = Context.User;
            logger.Debug($"User {Context.User.Identity.Name} delete message from {message.ToChatName}");
            await _message.DeleteMessage(message, isForMe);
        }

        public async Task<GroupModel> GetGroup(string groupId)
        {
            Thread.CurrentPrincipal = Context.User;
            return await _chat.GetGroup(groupId);
        }

        public async Task<MessageModel> UpdateMessage(MessageModel newMessage)
        {
            logger.Debug($"User {Context.User.Identity.Name} update message int chat {newMessage.ChatId}");
            return await _message.UpdateMessage(newMessage);
        }

        public async Task<MessageModel> GetLastMessage(string messageId)
        {
            return _message.GetLastMessage(messageId);
        }

        public async Task<MessageModel> ForwardMessage(string chatId, string messageId)
        {
            Thread.CurrentPrincipal = Context.User;
            logger.Debug($"User {Context.User.Identity.Name} forward message {messageId} int chat {chatId}");
            return await _message.ForwardMessage(chatId, messageId);
        }

        public async Task UpdateUserInfo(UserModel userInfo)
        {
            await _user.UpdateUserInfo(userInfo);
        }

        public async Task ReadMessages(string chatId)
        {
            Thread.CurrentPrincipal = Context.User;
            await _message.ReadMessages(chatId);
        }

        public async Task<string> CreateGroup(GroupModel groupModel) 
        {
            Thread.CurrentPrincipal = Context.User;
            logger.Debug($"User {Context.User.Identity.Name} create group {groupModel.Name}");
            return await _chat.CreateGroup(groupModel);
        }

        public async Task UpdateGroup(GroupModel groupModel)
        {
            logger.Debug($"User {Context.User.Identity.Name} update group {groupModel.Name}");
            await _chat.UpdateGroup(groupModel);
        }

        public async Task<bool> ChangePassword(ChangePasswordViewModel model)
        {
            Thread.CurrentPrincipal = Context.User;
            return await _user.ChangePassword(model);
        }
    }
}
