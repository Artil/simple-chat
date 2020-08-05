using ChatCore.Models;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace ChatDesktop.ViewModels.Dialogs
{
    public class ForwardMessageDialogVM : BaseDialogVM
    {
        private readonly IChatHub _chat;
        private string _messageId;
        public event Action<MessageModel> PostForwardMessage;

        private ICommand _forwardMessage;
        public ICommand ForwardMessageCommand => _forwardMessage == null ? _forwardMessage = new RelayCommand(ForwardMessageAction) : _forwardMessage;

        public ForwardMessageDialogVM(IChatHub chat, string text, string messageId):base(text)
        {
            _chat = chat;
            _messageId = messageId;
            GetUserChats();
        }

        private ObservableCollection<ChatListModel> _chatList;
        public ObservableCollection<ChatListModel> ChatList
        {
            get { return _chatList; }
            set
            {
                _chatList = value;
                OnPropertyChanged();
            }
        }

        private async void ForwardMessageAction(object obj)
        {
            var message = await _chat.ForwardMessage(obj.ToString(), _messageId);
            PostForwardMessage.Invoke(message);
        }

        public async void GetUserChats()
        {
            var chats = await _chat.GetUserChats();
            if (ReferenceEquals(chats, null))
                ChatList = new ObservableCollection<ChatListModel>();
            else
                ChatList = new ObservableCollection<ChatListModel>(chats);

            Client.GetSavedMedia(chats);
        }
    }
}
