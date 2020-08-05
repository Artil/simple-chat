using ChatCore.Factories;
using ChatCore.Models;
using ChatDesktop.Enums;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using ChatDesktop.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace ChatDesktop.ViewModels.SlideMenu
{
    public class ContactsVM : SlideMenuBase
    {
        private readonly IChatHub _chat;
        public ContactsVM(IChatHub chat)
        {
            _chat = chat;
            _chat.ConnectionStarted += GetUsers;
        }

        private ICommand _createGroup;
        public ICommand CreateGroupCommand => _createGroup == null ? _createGroup = new RelayCommand(CreateGroupAction) : _createGroup;

        public async void GetUsers()
        {
            var users = await _chat.GetContactList();
            Client.GetSavedMedia(users);
            if (ReferenceEquals(users, null))
                UserList = new ObservableCollection<UserListModel>();
            else
                UserList = new ObservableCollection<UserListModel>(users);
        }

        private ObservableCollection<UserListModel> _userList;
        public ObservableCollection<UserListModel> UserList
        {
            get => _userList;
            set
            {
                _userList = value;
                OnPropertyChanged();
            }
        }

        private UserListModel _userSelected;
        public UserListModel UserSelected
        {
            get { return _userSelected; }
            set
            {
                if (ReferenceEquals(_userSelected, value))
                    return;

                _userSelected = value;

                if(!ReferenceEquals(value, null))
                    ChangeBaseContentAction(value.Name);
                OnPropertyChanged();
            }
        }

        private void ChangeBaseContentAction(object obj)
        {
            if (obj is null)
                return;

            var user = new { UserName = obj as String };

            Client.ChangeBaseContent(BaseContentEnum.UserView, user);
            UserSelected = null;
        }

        private void CreateGroupAction(object obj)
        {
            Client.ChangeBaseContent(BaseContentEnum.GroupCreateView, null);
        }
    }
}
