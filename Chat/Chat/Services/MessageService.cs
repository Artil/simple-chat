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
    public class MessageService : UtilityService, IMessage
    {
        private readonly Context _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly UserManager<User> _userManager;
        private readonly IChat _chat;

        public MessageService(Context context, IHubContext<ChatHub> hubContext, UserManager<User> userManager, IChat chat) : base(context)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
            _chat = chat;
        }

        public IEnumerable<MessageModel> GetMessagesByChatId(string chatId, int count = 0)
        {
            var chat = _context.Chats.Find(chatId);

            if (ReferenceEquals(chat, null))
                return null;

            var messageList = new List<MessageModel>();

            var chatMessages = _context.Messages
                .Where(x => String.Equals(x.ChatId, chat.Id))
                .AsEnumerable()
                .Where(x => !IsMessageDeleted(x))
                .Select(y => new MessageModel()
                {
                    MessageId = y.Id,
                    ChatId = y.ChatId,
                    ToChatName = chat.ChatName ?? GetChatName(chat.Id),
                    Content = y.Content,
                    File = String.IsNullOrEmpty(y.File) || String.IsNullOrWhiteSpace(y.File) ? null : new FileModel() { FileFullName = y.FileDb.FilePath, IsImage = y.FileDb.IsImage, IsVoice = y.FileDb.IsAudio, IsVideo = y.FileDb.IsVideo },
                    IsMyMessage = String.Equals(y.User.UserName, Thread.CurrentPrincipal.Identity.Name) ? true : false,
                    Name = y.User.UserName,
                    DateCreate = y.Created
                });

            messageList.AddRange(chatMessages);

            var forwardMessages = _context.ForwardMessages.Where(x =>
                _context.Messages.Any(y => String.Equals(x.MessageId, y.Id) && String.Equals(chatId, y.ChatId)))
                .AsEnumerable()
                .Where(x => !IsMessageDeleted(x.Message))
                .Select(y => new MessageModel()
                {
                    ChatId = chatId,
                    Content = y.Message.Content,
                    File = String.IsNullOrEmpty(y.Message.File) || String.IsNullOrWhiteSpace(y.Message.File) ? null : new FileModel() { FileFullName = y.Message.FileDb.FilePath, IsImage = y.Message.FileDb.IsImage, IsVoice = y.Message.FileDb.IsAudio, IsVideo = y.Message.FileDb.IsVideo },
                    MessageId = y.Message.Id,
                    ForwardMessageId = y.Id,
                    DateCreate = y.Message.Updated ?? y.Created,
                    ToChatName = chat.ChatName ?? GetChatName(chat.Id),
                    IsMyMessage = String.Equals(y.User.UserName, Thread.CurrentPrincipal.Identity.Name) ? true : false,
                    IsForward = true,
                    Name = y.User.UserName,
                    ForwardUserName = y.Message.User.UserName
                });

            messageList.AddRange(forwardMessages);

            messageList = messageList.OrderByDescending(z => z.DateCreate).ToList();

            if(!(messageList.LastOrDefault() is null))
                messageList.LastOrDefault().IsLast = true;

            return messageList.Skip(count).Take(50);
        }

        public async Task<MessageModel> SendMessage(MessageModel message)
        {
            var user = await _userManager.FindByNameAsync(Thread.CurrentPrincipal.Identity.Name);

            if (ReferenceEquals(message.ChatId, null))
            {
                var companion = await _userManager.FindByNameAsync(message.ToChatName);
                if (!(await _chat.IsChatExist(user, companion, message)))
                    _chat.CreateNewChat(user, companion, message);
            }

            FileDb fileDb = null;
            if (!ReferenceEquals(message.File, null))
            {
                fileDb = new FileDb();
                fileDb.FilePath = message.File.FileFullName;
                fileDb.IsImage = message.File.IsImage;
                fileDb.IsAudio = message.File.IsVoice;
                fileDb.IsVideo = message.File.IsVideo;
                _context.Files.Add(fileDb);
                _context.Entry(fileDb).State = EntityState.Added;
            }

            var dbMessage = new Message()
            {
                ChatId = message.ChatId,
                Content = message.Content,
                UserId = user.Id,
                File = ReferenceEquals(message.File, null) ? null : fileDb?.Id,
                Created = DateTime.Now
            };

            _context.Messages.Add(dbMessage);
            _context.Entry(dbMessage).State = EntityState.Added;

            await _context.SaveChangesAsync();

            message.MessageId = dbMessage.Id;
            message.DateCreate = dbMessage.Created;

            var dbCurrentStatusMessage = new MessageStatus()
            {
                MessageId = dbMessage.Id,
                IsRead = true,
                UserId = user.Id,
            };
            _context.MessagesStatus.Add(dbCurrentStatusMessage);

            if ((await _context.Chats.FindAsync(message.ChatId)).ChatType == ChatEnum.ChatForTwo)
            {
                var chatForTwo = _context.ChatsForTwo.FirstOrDefault(x => String.Equals(x.ChatId, message.ChatId));
                var dbCompanionStatusMessage = new MessageStatus()
                {
                    MessageId = dbMessage.Id,
                    UserId = String.Equals(Thread.CurrentPrincipal.Identity.Name, chatForTwo.FirstUser.UserName) ? chatForTwo.SecondUser.Id : chatForTwo.FirstUser.Id
                };

                _context.MessagesStatus.Add(dbCompanionStatusMessage);
                message.ToChatName = String.Equals(user.UserName, chatForTwo.FirstUser.UserName) ? chatForTwo.FirstUser.UserName : chatForTwo.SecondUser.UserName;
                await _hubContext.Clients.User(dbCompanionStatusMessage.UserId).SendAsync("ReceiveMessage", message);
                message.ToChatName = user.UserName;
            }
            else
            {
                var chatForGroup = _context.ChatsForGroup.FirstOrDefault(x => String.Equals(x.ChatId, message.ChatId));
                var sorted = chatForGroup.GroupsUsers.Where(x => !String.Equals(user.Id, x.UserId)).Select(x => x.UserId);

                if(!chatForGroup.GroupsUsers.Any(x => String.Equals(user.Id, x.UserId)))
                {
                    _context.GroupsUsers.Add(new GroupsUsers() { GroupId = chatForGroup.Id, UserId = user.Id });

                    var newChat = new ChatListModel()
                    {
                        ChatId = chatForGroup.ChatId,
                        ChatName = chatForGroup.Chat.ChatName,
                        ChatEnum = ChatEnum.ChatForGroup
                    };

                    await _hubContext.Clients.User(user.Id).SendAsync("NewChat", newChat);
                }


                foreach (var userId in sorted)
                {
                    var dbCompanionStatusMessage = new MessageStatus()
                    {
                        MessageId = dbMessage.Id,
                        UserId = userId
                    };
                    _context.MessagesStatus.Add(dbCompanionStatusMessage);
                }
                await _hubContext.Clients.Users(sorted.ToList()).SendAsync("ReceiveMessage", message);
            }

            await _context.SaveChangesAsync();

            message.IsMyMessage = true;

            return message;
        }

        public async Task DeleteMessage(MessageModel message, bool isForMe = false)
        {
            if (ReferenceEquals(message.ForwardMessageId, null))
            {
                if (isForMe)
                    _context.MessagesStatus.FirstOrDefault(x => String.Equals(x.MessageId, message.MessageId)
                        && String.Equals(x.User.UserName, Thread.CurrentPrincipal.Identity.Name)).IsDeleted = true;
                else
                {
                    await _context.MessagesStatus
                        .Where(x => String.Equals(x.MessageId, message.MessageId))
                        .ForEachAsync(x => x.IsDeleted = true);
                }
            }
            else
            {
                if (isForMe)
                    _context.MessagesStatus.FirstOrDefault(x => String.Equals(x.ForwardId, message.ForwardMessageId)
                        && String.Equals(x.User.UserName, Thread.CurrentPrincipal.Identity.Name)).IsDeleted = true;
                else
                {
                    await _context.MessagesStatus
                        .Where(x => String.Equals(x.ForwardId, message.ForwardMessageId))
                        .ForEachAsync(x => x.IsDeleted = true);
                }
            }

            await _context.SaveChangesAsync();


            if((await _context.Chats.FindAsync(message.ChatId)).ChatType == ChatEnum.ChatForTwo)
            {
                var companion = await _userManager.FindByNameAsync(message.ToChatName);
                await _hubContext.Clients.User(companion.Id).SendAsync("MessageDeleted", message.MessageId);
            }
            else
            {
                var userList = _context.ChatsForGroup.FirstOrDefault(x =>
                    String.Equals(x.ChatId, message.ChatId))
                    .GroupsUsers.Where(x =>
                    !String.Equals(x.User.UserName, Thread.CurrentPrincipal.Identity.Name))
                    .Select(x => x.UserId)
                    .ToList();

                await _hubContext.Clients.Users(userList).SendAsync("MessageUpdated", message);
            }
        }

        public async Task<MessageModel> UpdateMessage(MessageModel newMessage)
        {
            var message = _context.Messages.FirstOrDefault(x => String.Equals(x.Id, newMessage.MessageId));
            message.Content = newMessage.Content;
            message.Updated = DateTime.Now;
            newMessage.UpdateTime = message.Updated;

            await _context.SaveChangesAsync();

            if ((await _context.Chats.FindAsync(newMessage.ChatId)).ChatType == ChatEnum.ChatForTwo)
            {
                var companion = await _userManager.FindByNameAsync(newMessage.ToChatName);
                await _hubContext.Clients.User(companion.Id).SendAsync("MessageUpdated", newMessage);
            }
            else
            {
                var userList = _context.ChatsForGroup.FirstOrDefault(x => 
                    String.Equals(x.ChatId, message.ChatId))
                    .GroupsUsers.Where(x =>
                    !String.Equals(x.UserId, message.UserId))
                    .Select(x => x.UserId)
                    .ToList();

                await _hubContext.Clients.Users(userList).SendAsync("MessageUpdated", newMessage);
            }

            return newMessage;
        }

        public MessageModel GetLastMessage(string messageId)
        {
            var message = _context.Messages.FirstOrDefault(x => String.Equals(x.Id, messageId));
            var lastMessage = _context.Messages
                .Where(x => String.Equals(x.ChatId, message.ChatId) && !String.Equals(x.Id, messageId))
                .OrderBy(y => y.Created)
                .ToList()
                .LastOrDefault(a => !IsMessageDeleted(a).Equals(true));
            return new MessageModel()
            {
                ChatId = lastMessage?.ChatId ?? message.ChatId,
                MessageId = lastMessage?.Id,
                Content = lastMessage?.Content,
                DateCreate = lastMessage?.Created
            };
        }

        public async Task<MessageModel> ForwardMessage(string chatId, string messageId)
        {
            var user = await _userManager.FindByNameAsync(Thread.CurrentPrincipal.Identity.Name);
            var chat = await _context.Chats.FindAsync(chatId);
            var message = await _context.Messages.FindAsync(messageId);

            var newForwardMessageDb = new ForwardChatMessage()
            {
                ChatId = chatId,
                UserId = user.Id,
                MessageId = messageId,
                Created = DateTime.Now,
            };

            _context.ForwardMessages.Add(newForwardMessageDb);
            _context.Entry(newForwardMessageDb).State = EntityState.Added;

            var sendMessage = new MessageModel()
            {
                ChatId = chatId,
                Content = message.Content,
                File = String.IsNullOrEmpty(message.File) || String.IsNullOrWhiteSpace(message.File) ? null : new FileModel() { FileFullName = message.FileDb.FilePath, IsImage = message.FileDb.IsImage },
                MessageId = messageId,
                ForwardMessageId = newForwardMessageDb.Id,
                DateCreate = newForwardMessageDb.Created,
                ToChatName = chat.ChatName ?? GetChatName(chat.Id),
                Name = user.UserName,
                PhotoPath = _context.UsersAccountInfo.FirstOrDefault(x => String.Equals(x.UserId, user.Id))?.FileDb?.FilePath,
                IsMyMessage = false,
                IsForward = true,
                ForwardUserName = message.User.UserName
            };

            var dbCurrentStatusMessage = new MessageStatus()
            {
                MessageId = message.Id,
                ForwardId = newForwardMessageDb.Id,
                UserId = message.UserId,
            };
            _context.MessagesStatus.Add(dbCurrentStatusMessage);

            if(chat.ChatType == ChatEnum.ChatForTwo)
            {
                var chatForTwo = _context.ChatsForTwo.FirstOrDefault(x => String.Equals(x.ChatId, chatId));
                var dbCompanionStatusMessage = new MessageStatus()
                {
                    MessageId = message.Id,
                    ForwardId = newForwardMessageDb.Id,
                    UserId = String.Equals(Thread.CurrentPrincipal.Identity.Name, chatForTwo.FirstUser.UserName) ? chatForTwo.SecondUser.Id : chatForTwo.FirstUser.Id
                };

                _context.MessagesStatus.Add(dbCompanionStatusMessage);
                await _hubContext.Clients.User(dbCompanionStatusMessage.UserId).SendAsync("ReceiveMessage", sendMessage);
            }
            else
            {
                var chatForGroup = _context.ChatsForGroup.FirstOrDefault(x => String.Equals(x.ChatId, chatId)).GroupsUsers.Where(x => !String.Equals(user.Id, x.UserId)).Select(x => x.UserId);

                foreach(var userId in chatForGroup)
                {
                    var dbCompanionStatusMessage = new MessageStatus()
                    {
                        MessageId = message.Id,
                        ForwardId = newForwardMessageDb.Id,
                        UserId = userId
                    };
                    _context.MessagesStatus.Add(dbCompanionStatusMessage);
                }
                await _hubContext.Clients.Users(chatForGroup.ToList()).SendAsync("ReceiveMessage", sendMessage);
            }

            await _context.SaveChangesAsync();

            sendMessage.IsMyMessage = true;

            return sendMessage;
        }

        private string GetChatName(string chatId)
        {
            var chat = _context.ChatsForTwo.FirstOrDefault(x => String.Equals(chatId, x.ChatId));
            return String.Equals(Thread.CurrentPrincipal.Identity.Name, chat.FirstUser.UserName) ? 
                chat.SecondUser.UserName : 
                chat.FirstUser.UserName;
        }

        public async Task ReadMessages(string chatId)
        {
            var user = await _userManager.FindByNameAsync(Thread.CurrentPrincipal.Identity.Name);
            var messages = _context.MessagesStatus.Where(x => String.Equals(x.Message.ChatId, chatId) && !x.IsRead && String.Equals(x.UserId, user.Id));
            var forwardMessages = _context.MessagesStatus.Where(x => String.Equals(x.ForwardMessage.ChatId, chatId) && !x.IsRead&& String.Equals(x.UserId, user.Id));

            foreach (var item in messages)
            {
                item.IsRead = true;
            }

            foreach (var item in forwardMessages)
            {
                item.IsRead = true;
            }

            _context.SaveChanges();
        }
    }
}
