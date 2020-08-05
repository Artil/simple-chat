using ChatDesktop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ToastNotifications.Core;

namespace ChatDesktop.Notifications
{
    public class NotifyMessage : NotificationBase, INotifyPropertyChanged
    {
        public event EventHandler Clicked;
        private MessageNotification notificationView = null;

        public override NotificationDisplayPart DisplayPart => notificationView;

        public NotifyMessage(string title, string user, string color, string photoPath, string message, MessageOptions messageOptions) : base(message, messageOptions)
        {
            Title = title;
            Message = message;
            User = user;
            Color = color;
            PhotoPath = photoPath;

            if (String.Equals(title, user))
                IsChatForTwo = true;


            notificationView = new MessageNotification();
            notificationView.Bind(this);
            notificationView.MouseDown += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
            {
                Clicked?.Invoke(this, EventArgs.Empty);
            };
        }

        public string ShortName => User.Substring(0,1).ToUpper();

        private string _color;
        public string Color
        {
            get => _color;
            set
            {
                if (String.Equals(_color, value))
                    return;

                _color = value;
                OnPropertyChanged();
            }
        }

        private string _photoPath;
        public string PhotoPath
        {
            get => _photoPath;
            set
            {
                if (String.Equals(_photoPath, value))
                    return;

                _photoPath = value;
                OnPropertyChanged();
            }
        }

        private bool _isChatForTwo;
        public bool IsChatForTwo
        {
            get => _isChatForTwo;
            set
            {
                _isChatForTwo = value;
                OnPropertyChanged();
            }
        }

        private bool _isForGroup;
        public bool IsForGroup
        {
            get => _isForGroup;
            set
            {
                if (String.Equals(_isForGroup, value))
                    return;

                _isForGroup = value;
                OnPropertyChanged();
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _user;
        public string User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
