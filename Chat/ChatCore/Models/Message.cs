using ChatCore.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatCore.Models
{
    public class MessageModel : BaseUserModel
    {
        public string MessageId { get; set; }
        public string ForwardMessageId { get; set; }
        public string ChatId { get; set; }
        public string ForwardUserName { get; set; }
        public string ToChatName { get; set; }
        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                if (String.Equals(_content, value))
                    return;

                _content = value;
                OnPropertyChanged();
            }
        }
        public FileModel File { get; set; }
        public bool? IsMyMessage { get; set; }
        public bool? IsForward { get; set; }
        public bool IsLast { get; set; }
        public DateTime? DateCreate { get; set; }
        private DateTime? _updateTime;

        public DateTime? UpdateTime
        {
            get => _updateTime;
            set
            {
                _updateTime = value;
                OnPropertyChanged();
            }
        }
    }
}
