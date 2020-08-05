using ChatCore.Enums;
using ChatCore.Factories;
using ChatCore.Models;
using ChatDesktop.Enums;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using ChatDesktop.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ChatCore.Extensions;

namespace ChatDesktop.ViewModels.SlideMenu
{
    public class AddUserVM : SlideMenuBase
    {
        private readonly IChatHub _chat;
        private readonly IFile _file;
        private ChatVM baseContent;

        public AddUserVM(IChatHub chat, IFile file, IAbstractFactory<ChatVM> abstractFactory)
        {
            _chat = chat;
            _file = file;
            baseContent = abstractFactory.GetInstance();
            baseContent.NewChatCreated += RemoveFindedUser;
        }

        private ICommand _search;
        public ICommand SearchCommand => _search == null ? _search = new RelayCommand(SearchAction) : _search;

        private SearchByEnum _searchCondition;
        public SearchByEnum SearchCondition
        {
            get => _searchCondition;
            set
            {
                if (String.Equals(_searchCondition, value))
                    return;

                _searchCondition = value;
                OnPropertyChanged();
            }
        }

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (String.Equals(_searchQuery, value))
                    return;

                _searchQuery = value;
                OnPropertyChanged();
            }
        }

        private bool _isUsersListVis;
        public bool IsUsersListVis
        {
            get => _isUsersListVis;
            set
            {
                if (_isUsersListVis.Equals(value))
                    return;
                _isUsersListVis = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<UserListModel> _userQueryList;
        public ObservableCollection<UserListModel> UserQueryList
        {
            get => _userQueryList;
            set
            {
                _userQueryList = value;
                OnPropertyChanged();
            }
        }

        private UserListModel _userSelected;
        public UserListModel UserSelected
        {
            get => _userSelected;
            set
            {
                if (ReferenceEquals(_userSelected, value))
                    return;

                _userSelected = value;
                if (!ReferenceEquals(value, null))
                    GoToChatForTwo();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<GroupModel> _groupQueryList;
        public ObservableCollection<GroupModel> GroupQueryList
        {
            get => _groupQueryList;
            set
            {
                _groupQueryList = value;
                OnPropertyChanged();
            }
        }

        private GroupModel _groupSelected;
        public GroupModel GroupSelected
        {
            get => _groupSelected;
            set
            {
                if (ReferenceEquals(_groupSelected, value))
                    return;

                _groupSelected = value;
                if (!ReferenceEquals(value, null))
                    GoToChatForGroup();
                OnPropertyChanged();
            }
        }

        private void GoToChatForTwo()
        {
            var obj = new
            {
                UserSelected.Color,
                ChatEnum = ChatEnum.ChatForTwo,
                ChatId = (String)null,
                ChatName = UserSelected.Name,
                UserSelected.PhotoPath
            };

            Client.ChangeBaseContent(BaseContentEnum.ChatView, obj);
            UserSelected = null;
        }

        private void GoToChatForGroup()
        {
            var obj = new
            {
                GroupSelected.Color,
                ChatEnum = ChatEnum.ChatForGroup,
                ChatId = GroupSelected.GroupId,
                ChatName = GroupSelected.Name,
                GroupSelected.PhotoPath
            };

            Client.ChangeBaseContent(BaseContentEnum.ChatView, obj);
            GroupSelected = null;
        }

        public IEnumerable<object> SearchByList => Enum.GetValues(typeof(SearchByEnum)).Cast<SearchByEnum>().Select(x => new { key = x, value = x.GetDisplayDescription() });

        private string lastSearch;
        private SearchByEnum lastEnum;

        public async void SearchAction(object obj)
        {
            if (String.IsNullOrEmpty(SearchQuery) || 
                (String.Equals(lastSearch, SearchQuery) && SearchCondition.Equals(lastEnum)))
                return;

            lastSearch = SearchQuery;
            lastEnum = SearchCondition;


            if (SearchCondition.Equals(SearchByEnum.Group))
            {
                var groupList = await _chat.GetRequestGroupList(SearchQuery);
                await  _file.GetPhotos(groupList);
                GroupQueryList = new ObservableCollection<GroupModel>(groupList);
                IsUsersListVis = false;
            }
            else
            {
                var usersList = await _chat.GetRequestUserList(SearchQuery, SearchCondition);
                await _file.GetPhotos(usersList);
                UserQueryList = new ObservableCollection<UserListModel>(usersList);
                IsUsersListVis = true;
            }
        }

        public void RemoveFindedUser(string chatIdent)
        {
            var userResult = UserQueryList.FirstOrDefault(x => String.Equals(x.Name, chatIdent));
            if(ReferenceEquals(userResult, null))
            {
                var groupResult = GroupQueryList.FirstOrDefault(x => String.Equals(x.GroupId, chatIdent));
                if (!ReferenceEquals(groupResult, null))
                    GroupQueryList.Remove(groupResult);
            }
            else
                UserQueryList.Remove(userResult);
        }
    }
}
