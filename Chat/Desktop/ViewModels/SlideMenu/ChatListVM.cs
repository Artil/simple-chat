using ChatCore.Factories;
using ChatCore.Models;
using ChatDesktop.Enums;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using ChatDesktop.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ChatDesktop.ViewModels.SlideMenu
{
    public class ChatListVM : SlideMenuBase
    {
        private readonly IChatHub _chat;
        private readonly IFile _file;
        private ChatVM baseContent;

        public ChatListVM(IChatHub chat, IAbstractFactory<ChatVM> chatFactory, IFile file)
        {
            _chat = chat;
            _file = file;
            baseContent = chatFactory.GetInstance();

            _chat.ConnectionStarted += GetUserChats;
            _chat.NewChat += AddChat;
            _chat.NewCompanionMessage += AddMessage;
            baseContent.NewMyMessage += AddMessage;
            _chat.MessageUpdated += UpdateMessage;
            _chat.MessageDeleted += GetLastMessage;
            baseContent.RemoveMyMessage += GetLastMessage;
        }

        private ICommand _removeChat;
        public ICommand RemoveChatCommand => _removeChat == null ? _removeChat = new RelayCommand(RemoveChatAction) : _removeChat;

        private ObservableCollection<ChatListModel> _chatList;
        public ObservableCollection<ChatListModel> ChatList
        {
            get => _chatList;
            set
            {
                _chatList = value;
                OnPropertyChanged();
            }
        }

        private ChatListModel _chatSelected;
        public ChatListModel ChatSelected
        {
            get => _chatSelected; 
            set
            {
                if (!ReferenceEquals(_chatSelected, value))
                    _chatSelected = value;
                if(!ReferenceEquals(value, null))
                    ChangeBaseWindow();
                OnPropertyChanged();
            }
        }

        private bool _scrollVisible = true;
        public bool ScrollVisible
        {
            get => _scrollVisible;
            set
            {
                if (_scrollVisible.Equals(value))
                    return;

                    _scrollVisible = value;
                OnPropertyChanged();
            }
        }

        public void ChangeBaseWindow()
        {
            Client.ChangeBaseContent(BaseContentEnum.ChatView, ChatSelected);
            ChatSelected.NewMessagesCount = 0;
            ChatSelected = null;
        }

        public async void GetUserChats()
        {
            var chats = await _chat.GetUserChats();

            await _file.GetPhotos(chats);

            if(ReferenceEquals(chats, null))
                ChatList = new ObservableCollection<ChatListModel>();
            else
                ChatList = new ObservableCollection<ChatListModel>(chats);
        }

        private void AddMessage(MessageModel message)
        {
            var chat = ChatList.FirstOrDefault(x => x.ChatId == message.ChatId);
            chat.Created = message.DateCreate ?? DateTime.Now;
            chat.Message = message.Content;
            if(message.IsMyMessage != true && !String.Equals(baseContent.ChatId, message.ChatId))
                chat.NewMessagesCount++;
        }

        private void UpdateMessage(MessageModel updatedMessage)
        {
            var chat = ChatList.FirstOrDefault(x => x.ChatId == updatedMessage.ChatId);

            if (!String.Equals(chat?.MessageId, updatedMessage.MessageId))
                return;

            chat.Created = updatedMessage.UpdateTime;
            chat.Message = updatedMessage.Content;
        }

        private async void GetLastMessage(MessageModel delMessage)
        {
            if (!ChatList.Any(x => String.Equals(x.MessageId, delMessage.MessageId)) || !ChatList.Any(x => String.Equals(x.MessageId, delMessage.ForwardMessageId)))
                return;

            var message = await _chat.GetLastMessage(delMessage.MessageId);
            var chat = ChatList.FirstOrDefault(x => x.ChatId == message.ChatId);

            chat.MessageId = message?.MessageId;
            chat.Created = message?.DateCreate;
            chat.Message = message?.Content;
        }

        private async void AddChat(ChatListModel chat)
        {
            chat.PhotoPath = await _file.GetFile(chat.PhotoPath);
            ChatList.Add(chat);
        }

        public async void RemoveChatAction(object obj)
        {
            var chat = obj as ChatListModel;
            ChatList.Remove(chat);
        }
    }
}
