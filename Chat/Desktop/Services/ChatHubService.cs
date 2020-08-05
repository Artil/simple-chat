using ChatCore.Enums;
using ChatCore.Models;
using ChatDesktop.Helpers;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using ChatDesktop.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ChatDesktop.Notifications;
using ChatDesktop.Views.BaseWindow;
using System.Linq;
using ChatDbCore.Account;

namespace ChatDesktop.Services
{
    public class ChatHubService : IChatHub
    {
        private HubConnection connection;
        private readonly IConfiguration _config;
        private readonly string url;
        private readonly MainWindow _mainWindow;
        private Notifier notifier;

        public event Action ConnectionStarted;
        public event Action<MessageModel> NewCompanionMessage;
        public event Action<ChatListModel> NewChat;
        public event Action<MessageModel> MessageDeleted;
        public event Action<MessageModel> MessageUpdated;

        SoundPlayer player = new SoundPlayer(Application.GetResourceStream(new Uri(@"pack://application:,,,/Resources/music/ReceiveMessage.wav")).Stream);

        public ChatHubService(IConfiguration config, MainWindow mainWindow)
        {
            _config = config;
            url = _config.GetSection("Connection").GetSection("chat").Value;
            _mainWindow = mainWindow;

            notifier = new Notifier(conf =>
            {
                conf.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(5), MaximumNotificationCount.FromCount(5));
                conf.PositionProvider = new PrimaryScreenPositionProvider(Corner.BottomRight, 10, 10);
                conf.Dispatcher = Dispatcher.CurrentDispatcher;
            });
        }

        public async void ChatHubManager()
        {
            connection = new HubConnectionBuilder()
                .WithUrl(url, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(Client.AccessToken);
                })
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            RegisterEvents();

            try
            {
                await Connect();
                ConnectionStarted?.Invoke();
            }
            catch
            {
            }
        }

        public async Task Connect()
        {
            await connection.StartAsync();
        }

        public async Task<IEnumerable<UserListModel>> GetRequestUserList(string query, SearchByEnum searchBy)
        {
            return await connection.InvokeAsync<IEnumerable<UserListModel>>("SearchUsersRequest", query, searchBy);
        }

        public async Task<IEnumerable<GroupModel>> GetRequestGroupList(string query)
        {
            return await connection.InvokeAsync<IEnumerable<GroupModel>>("SearchGroupsRequest", query);
        }

        public async Task<IEnumerable<UserListModel>> GetContactList()
        {
            return await connection.InvokeAsync<IEnumerable<UserListModel>>("GetContacts");
        }

        public async Task<GroupModel> GetGroup(string groupId)
        {
            return await connection.InvokeAsync<GroupModel>("GetGroup", groupId);
        }

        public async Task UpdateGroup(GroupModel group)
        {
            await connection.InvokeAsync("UpdateGroup", group);
        }

        public async Task<bool> IsUserOnlineCheck(string userName)
        {
            return await connection.InvokeAsync<bool>("IsUserOnlineCheck", userName);
        }

        public async Task<IEnumerable<MessageModel>> GetMessages(string chatId, int count = 0)
        {
            return await connection.InvokeAsync<IEnumerable<MessageModel>>("GetMessages", chatId, count);
        }

        public async Task<MessageModel> SendMessage(MessageModel message)
        {
            return await connection.InvokeAsync<MessageModel>("SendMessage", message);
        }

        public async Task<IEnumerable<ChatListModel>> GetUserChats()
        {
            return await connection.InvokeAsync<IEnumerable<ChatListModel>>("GetChats");
        }

        public async Task DeleteMessage(MessageModel message, bool isForMe = false)
        {
            await connection.InvokeAsync("DeleteMessage", message, isForMe);
        }

        public async Task<MessageModel> UpdateMessage(MessageModel message)
        {
            return await connection.InvokeAsync<MessageModel>("UpdateMessage", message);
        }

        public async Task<MessageModel> GetLastMessage(string messageId)
        {
            return await connection.InvokeAsync<MessageModel>("GetLastMessage", messageId);
        }

        public async Task<MessageModel> ForwardMessage(string messageId, string chatId)
        {
            return await connection.InvokeAsync<MessageModel>("ForwardMessage", messageId, chatId);
        }

        public async Task<UserModel> GetUser(string userName = null)
        {
            return await connection.InvokeAsync<UserModel>("GetUser", userName);
        }

        public async Task UpdateUserInfo(UserModel userInfo)
        {
            await connection.InvokeAsync("UpdateUserInfo", userInfo);
        }

        public async Task ReadMessages(string chatId)
        {
            await connection.InvokeAsync("ReadMessages", chatId);
        }

        public async Task<string> CreateGroup(GroupModel groupModel)
        {
            return await connection.InvokeAsync<string>("CreateGroup", groupModel);
        }

        public async Task<bool> ChangePassword(ChangePasswordViewModel changePasswordVM)
        {
            return await connection.InvokeAsync<bool>("ChangePassword", changePasswordVM);
        }

        public void RegisterEvents()
        {
            connection.On<ChatListModel>("NewChat", (chat) =>
            {
                NewChat?.Invoke(chat);
            });

            connection.On<MessageModel>("ReceiveMessage", (message) =>
            {
                message.IsMyMessage = false;
                NewCompanionMessage?.Invoke(message);
                if(!Client.Notifications.Any(x => String.Equals(message.ChatId, x)) 
                && !Properties.UserSettings.Default.AllNotifications)
                    Notify(message);
            });

            connection.On<MessageModel>("MessageDeleted", (message) =>
            {
                MessageDeleted?.Invoke(message);
            });

            connection.On<MessageModel>("MessageUpdated", (message) =>
            {
                MessageUpdated?.Invoke(message);
            });
        }

        private void Notify(MessageModel message)
        {
            if (!_mainWindow.IsActive)
                notifier.ShowNotification(message.ToChatName, message.Name, message.Color, message.PhotoPath, GetContent(message));

            FlashWindowHelper.FlashWindow(_mainWindow);
            player.Play();
        }

        private string GetContent(MessageModel message)
        {
            if (!ReferenceEquals(message.ForwardMessageId, null))
                return "Forwared message";

            if (!ReferenceEquals(message.File, null))
                return $"Send file - {message.File.FileName}";

            return message.Content;
        }
    }
}
