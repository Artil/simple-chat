using ChatCore.Enums;
using ChatCore.Models;
using ChatDbCore;
using ChatDbCore.Account;
using ChatDbCore.ChatModels;
using ChatServer.Hubs;
using ChatServer.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer.Services
{
    public class ChatService : UtilityService, IChat
    {
        private readonly Context _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly UserManager<User> _userManager;

        public ChatService(Context context, IHubContext<ChatHub> hubContext, UserManager<User> userManager) : base(context)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public async Task<IEnumerable<ChatListModel>> GetChats()
        {
            var chats = new List<ChatListModel>();
            var user = await _userManager.FindByNameAsync(Thread.CurrentPrincipal.Identity.Name);

            var chatForTwo = _context.ChatsForTwo.Where(x => x.FirstUserId == user.Id || x.SecondUserId == user.Id);

            var chatForGroup = _context.GroupsUsers.Where(x => x.UserId == user.Id);

            await chatForTwo.ForEachAsync(x =>
            {
                var messagesCount = _context.MessagesStatus.Where(y => y.Message.ChatId == x.ChatId && !y.IsRead && String.Equals(y.UserId, user.Id)).Count();
                var messages = _context.Messages.Where(y => String.Equals(y.ChatId, x.ChatId))
                    .ToList()
                    .Where(a => !IsMessageDeleted(a));

                var forwardMessage = _context.ForwardMessages.Where(y => String.Equals(y.ChatId, x.ChatId))
                    .ToList()
                    .Where(a => !IsMessageDeleted(a.Message))
                    .Select(b => new Message() { Id = b.Id, Content = b.Message.Content, Created = b.Created });

                var lastMessage = messages.Concat(forwardMessage).OrderBy(z => z.Created).LastOrDefault();

                chats.Add(new ChatListModel()
                {
                    ChatId = x.ChatId,
                    ChatName = String.Equals(x.SecondUserId, user.Id) ? x.FirstUser.UserName : x.SecondUser.UserName,
                    PhotoPath = _context.UsersAccountInfo.FirstOrDefault(y => y.UserId == (String.Equals(x.SecondUserId, user.Id) ? x.FirstUserId : x.SecondUserId))?.FileDb?.FilePath,
                    ChatEnum = ChatEnum.ChatForTwo,
                    MessageId = lastMessage?.Id,
                    Message = lastMessage?.Content,
                    Created = lastMessage?.Created,
                    NewMessagesCount = messagesCount
                });
            });

            await chatForGroup.ForEachAsync(x =>
            {
                var messagesCount = _context.MessagesStatus.Where(y => y.Message.ChatId == x.ChatForGroup.ChatId && !y.IsRead && String.Equals(y.UserId, user.Id)).Count();
                var message = _context.Messages.Where(y => String.Equals(y.ChatId, x.ChatForGroup.ChatId))
                    .OrderBy(z => z.Created)
                    .ToList()
                    .LastOrDefault(a => !IsMessageDeleted(a));

                chats.Add(new ChatListModel()
                {
                    ChatId = x.ChatForGroup.ChatId,
                    ChatName = x.ChatForGroup.Chat.ChatName,
                    PhotoPath = x.ChatForGroup.FileDb?.FilePath,
                    ChatEnum = ChatEnum.ChatForGroup,
                    MessageId = message?.Id,
                    Message = message?.Content,
                    Created = message?.Created,
                    NewMessagesCount = messagesCount
                });
            });

            return chats;
        }

        public async Task<bool> IsChatExist(User user, User companion, MessageModel message)
        {
            var existChat = _context.ChatsForTwo.FirstOrDefault(x =>
                    (String.Equals(x.FirstUserId, user.Id) && String.Equals(x.SecondUserId, companion.Id)) ||
                    (String.Equals(x.FirstUserId, companion.Id) && String.Equals(x.SecondUserId, user.Id))
                    && x.Chat.IsDeleted
                );

            if (ReferenceEquals(existChat, null))
                return false;

            existChat.Chat.IsDeleted = false;
            _context.SaveChanges();

            message.ChatId = existChat.ChatId;

            var newChat = new ChatListModel()
            {
                ChatId = existChat.Id,
                ChatName = companion.UserName,
                ChatEnum = ChatEnum.ChatForTwo
            };

            await _hubContext.Clients.User(user.Id).SendAsync("NewChat", newChat);
            newChat.ChatName = user.UserName;
            await _hubContext.Clients.User(companion.Id).SendAsync("NewChat", newChat);

            return true;
        }

        public async void CreateNewChat(User user, User companion, MessageModel message)
        {
            var chat = new Chat()
            {
                ChatType = ChatEnum.ChatForTwo,
                CreatorId = user.Id,
                Created = DateTime.Now
            };

            var chatForTwo = new ChatForTwo()
            {
                FirstUserId = user.Id,
                SecondUserId = companion.Id,
                Chat = chat
            };

            _context.Chats.Add(chat);
            _context.ChatsForTwo.Add(chatForTwo);
            _context.Entry(chat).State = EntityState.Added;
            _context.SaveChanges();

            // logger.Debug($"User {Context.User.Identity.Name} create chat {chat.Id}, chat type - {chat.ChatType.ToString()}");

            message.ChatId = chat.Id;

            var newChat = new ChatListModel()
            {
                ChatId = chat.Id,
                ChatName = companion.UserName,
                ChatEnum = ChatEnum.ChatForTwo
            };

            await _hubContext.Clients.User(user.Id).SendAsync("NewChat", newChat);
            newChat.ChatName = user.UserName;
            await _hubContext.Clients.User(companion.Id).SendAsync("NewChat", newChat);
        }

        public bool IsUsersInChat(User user, string currentUser)
        {
            return !_context.ChatsForTwo.Any(y =>
            ((String.Equals(y.FirstUser.UserName, currentUser) && String.Equals(y.SecondUser.UserName, user.UserName))
            || (String.Equals(y.FirstUser.UserName, user.UserName) && String.Equals(y.SecondUser.UserName, currentUser)))
            && !y.Chat.IsDeleted);
        }

        public async Task<string> CreateGroup(GroupModel groupModel)
        {
            var user = await _userManager.FindByNameAsync(Thread.CurrentPrincipal.Identity.Name);
            List<string> userList = new List<string>();
            userList.Add(user.Id);

            var chat = new Chat { ChatName = groupModel.Name, ChatType = ChatEnum.ChatForGroup, CreatorId = user.Id, Created = DateTime.Now };
            _context.Chats.Add(chat);
            _context.Entry(chat).State = EntityState.Added;

            var chatForGroup = new ChatForGroup()
            {
                ChatId = chat.Id,
                IsPublic = groupModel.IsPublic,
                CreatorMode = groupModel.CreatorMode
                
            };

            AddGroupPhoto(groupModel, chatForGroup);

            _context.ChatsForGroup.Add(chatForGroup);
            _context.Entry(chatForGroup).State = EntityState.Added;

            _context.GroupsUsers.Add(new GroupsUsers() { GroupId = chatForGroup.Id, UserId = user.Id });

            foreach (var item in groupModel.Users)
            {
                var id = (await _userManager.FindByNameAsync(item)).Id;
                userList.Add(id);
                _context.GroupsUsers.Add(new GroupsUsers() { GroupId = chatForGroup.Id, UserId = id });
            }

            await _context.SaveChangesAsync();

            var newChat = new ChatListModel()
            {
                ChatId = chat.Id,
                ChatName = groupModel.Name,
                ChatEnum = ChatEnum.ChatForGroup,
                PhotoPath = groupModel.PhotoPath
            };

            await _hubContext.Clients.Users(userList).SendAsync("NewChat", newChat);
            return chat.Id;
        }

        public IEnumerable<GroupModel> SearchGroupsRequest(string query)
        {
            return _context.ChatsForGroup
                .Where(x => x.Chat.ChatName.ToUpper().Contains(query.ToUpper()) && x.IsPublic)
                .Select(x => new GroupModel { GroupId = x.ChatId, Name = x.Chat.ChatName, IsPublic = x.IsPublic, UsersCount = x.GroupsUsers.Count() });
        }

        public async Task<GroupModel> GetGroup(string groupId)
        {
            var group = _context.ChatsForGroup.FirstOrDefault(x => String.Equals(x.ChatId, groupId));
            var users = group.GroupsUsers.Select(x => x.User.UserName);
            return new GroupModel { IsPublic = group.IsPublic, GroupId = groupId, Users = users, UsersCount = users.Count(), Name = group.Chat.ChatName };
        }

        public async Task UpdateGroup(GroupModel group)
        {
            var chatForGroup = _context.ChatsForGroup.FirstOrDefault(x => String.Equals(x.ChatId, group.GroupId));

            AddGroupPhoto(group, chatForGroup);

            chatForGroup.IsPublic = group.IsPublic;
            chatForGroup.CreatorMode = group.CreatorMode;

            foreach (var item in group.Users)
            {
                var user = chatForGroup.GroupsUsers.FirstOrDefault(x => String.Equals(x.User.UserName, item));
                if (user is null)
                    _context.GroupsUsers.Add(new GroupsUsers() { GroupId = chatForGroup.Id, UserId = (await _userManager.FindByNameAsync(item)).Id });

                if (user.IsRemoved)
                    user.IsRemoved = false;
            }
        }

        private async Task AddGroupPhoto(GroupModel group, ChatForGroup chatForGroup)
        {
            if (!String.IsNullOrEmpty(group.PhotoPath))
            {
                if (chatForGroup.FileDb is null)
                {
                    chatForGroup.FileDb = new FileDb();
                    chatForGroup.FileDb.IsImage = true;
                }
                chatForGroup.FileDb.FilePath = group.PhotoPath;
            }
        }
    }
}
