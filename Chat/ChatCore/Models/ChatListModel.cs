using ChatCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatCore.Models
{
    public class ChatListModel : BaseUserModel
    {
        public string ChatId { get; set; }

        private string _chatName;
        public string ChatName 
        {
            get => _chatName;
            set
            {
                if (String.Equals(_chatName, value))
                    return;

                _chatName = value;
                Name = value;
                OnPropertyChanged();
            }
        }

        public ChatEnum ChatEnum { get; set; }

        public string MessageId { get; set; }

        private string message;
        public string Message 
        {
            get => message;
            set
            {
                if (String.Equals(message, value))
                    return;

                message = value;
                OnPropertyChanged();
            }
        }

        private DateTime? created;
        public DateTime? Created 
        {
            get => created;
            set
            {
                if (created.Equals(value))
                    return;

                created = value;
                OnPropertyChanged();
            }
        }

        private int newMessagesCount;
        public int NewMessagesCount 
        {
            get => newMessagesCount;
            set
            {
                if (newMessagesCount == value)
                    return;

                newMessagesCount = value;
                OnPropertyChanged();
            }
        }
    }
}
