using ChatCore.Enums;
using ChatCore.Models;
using ChatDesktop.Enums;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatDesktop.ViewModels.Base
{
    public class GroupCreateVM : BaseContentVM
    {
        private readonly IChatHub _chat;
        private readonly IFile _file;
        public GroupCreateVM(IChatHub chat, IFile file)
        {
            _chat = chat;
            _file = file;
        }

        private SavedGroupModel savedGroup;

        private ICommand _createGroup;
        public ICommand CreateGroupCommand => _createGroup == null ? _createGroup = new RelayCommand(CreateGroupAction) : _createGroup;

        private ICommand _updateGroup;
        public ICommand UpdateGroupCommand => _updateGroup == null ? _updateGroup = new RelayCommand(UpdateGroupAction) : _updateGroup;

        private ICommand _removeGroup;
        public ICommand RemoveGroupCommand => _removeGroup == null ? _removeGroup = new RelayCommand(RemoveGroupAction) : _removeGroup;

        private ICommand _changeImage;
        public ICommand ChangeImageCommand => _changeImage == null ? _changeImage = new RelayCommand(ChangeImageAction) : _changeImage;

        private ObservableCollection<UserListModel> _avaibleUserList;
        public ObservableCollection<UserListModel> AvaibleUserList
        {
            get => _avaibleUserList;
            set
            {
                _avaibleUserList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<UserListModel> _userList = new ObservableCollection<UserListModel>();
        public ObservableCollection<UserListModel> UserList
        {
            get => _userList;
            set
            {
                _userList = value;
                OnPropertyChanged();
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded.Equals(value))
                    return;

                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        private string _chatName;
        [Required]
        public string ChatName
        {
            get => _chatName;
            set
            {
                if (String.Equals(_chatName, value))
                    return;

                _chatName = value;
                OnPropertyChanged("ShortName");
                OnPropertyChanged();
            }
        }

        private string _chatId;
        public string ChatId
        {
            get => _chatId;
            set
            {
                if (String.Equals(_chatId, value))
                    return;

                _chatId = value;
                OnPropertyChanged();
            }
        }

        private bool _isPublic;
        public bool IsPublic
        {
            get => _isPublic;
            set
            {
                if (_isPublic.Equals(value))
                    return;

                _isPublic = value;
                OnPropertyChanged();
            }
        }

        private bool _creatorMode;
        public bool CreatorMode
        {
            get => _creatorMode;
            set
            {
                if (_creatorMode.Equals(value))
                    return;

                _creatorMode = value;
                OnPropertyChanged();
            }
        }

        public string _color;
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

        public string _photoPath;
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

        private FileModel _fileContent;
        public FileModel FileContent
        {
            get => _fileContent;
            set
            {
                _fileContent = value;
                OnPropertyChanged();
            }
        }

        public string ShortName => ChatName?.Substring(0, 1).ToUpper();

        public async void GetUsers()
        {
            var users = (await _chat.GetContactList()).Where(x => !UserList.Any(y => String.Equals(y.Name, x.Name)));
            Client.GetSavedMedia(users);
            if (ReferenceEquals(users, null))
                AvaibleUserList = new ObservableCollection<UserListModel>();
            else
                AvaibleUserList = new ObservableCollection<UserListModel>(users);

            if (!String.Equals(ChatId, null))
                savedGroup.AvaibleUserList = new ObservableCollection<UserListModel>(users);
        }

        private async void CreateGroupAction(object obj)
        {
            if (!IsValid)
                return;

            if (!String.IsNullOrEmpty(ChatId))
                return;

            await SendPhoto();

            var group = new GroupModel()
            {
                Name = ChatName,
                IsPublic = IsPublic,
                PhotoPath = PhotoPath,
                CreatorMode = CreatorMode,
            };

            group.Users = UserList.Select(x => x.Name);

            group.GroupId = await _chat.CreateGroup(group);
            await GetPhoto(PhotoPath);
            GoToGroup(group);
        }

        private void GoToGroup(GroupModel group)
        {
            var obj = new
            {
                group.Color,
                ChatEnum = ChatEnum.ChatForGroup,
                ChatId = group.GroupId,
                ChatName = group.Name,
                PhotoPath
            };

            Client.ChangeBaseContent(BaseContentEnum.ChatView, obj);
        }

        private async void GetChat()
        {
            var group = await _chat.GetGroup(ChatId);
            Client.GetSavedMedia(new[] { group });
            Client.SetValues(this, group);

            UserList = new ObservableCollection<UserListModel>(group.Users.Select(x => new UserListModel() { Name = x }));

            Client.SetValues(savedGroup, this);
            savedGroup.UserList = new ObservableCollection<UserListModel>(UserList);
        }

        private async void UpdateGroupAction(object obj)
        {
            if (String.IsNullOrEmpty(ChatId))
                return;

            await SendPhoto();

            var group = new GroupModel()
            {
                Name = ChatName,
                PhotoPath = PhotoPath,
                IsPublic = IsPublic,
                CreatorMode = CreatorMode,
                GroupId = ChatId,
                Users = UserList.Select(x => x.Name),
            };

            await _chat.UpdateGroup(group);
            await GetPhoto(PhotoPath);
        }

        private async Task SendPhoto()
        {
            if (!String.Equals(PhotoPath, null))
            {
                try
                {
                    var url = $"/SendChatPhoto?chatId={ChatId}";
                    PhotoPath = await _file.SendFileToApi(url, new FileModel { FileFullName = Path.GetFileName(PhotoPath), FileContent = File.ReadAllBytes(PhotoPath) });
                }
                catch { }
            }
        }

        private async Task GetPhoto(string path)
        {
            PhotoPath = await _file.GetFile(path);

            if (Client.UsersMedia.Any(x => x.Key == ChatName))
                Client.UsersMedia.Remove(ChatName);

            Client.GetSavedMedia(new[] { new GroupModel() { Name = ChatName, PhotoPath = PhotoPath } });
        }

        private void RemoveGroupAction(object obj)
        {
            Client.SetValues(this, savedGroup);
            AvaibleUserList = new ObservableCollection<UserListModel>(savedGroup.AvaibleUserList);
            UserList = new ObservableCollection<UserListModel>(savedGroup.UserList);
        }

        private async void ChangeImageAction(object obj)
        {
            var openFileDialog = new CommonOpenFileDialog();

            openFileDialog.Filters.Add(new CommonFileDialogFilter("All Images Files", "*.png;*.jpeg;*.jpg;*.bmp;*.tiff;*.tif"));
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                PhotoPath = openFileDialog.FileName;
        }

        public override void OnSelected()
        {
            savedGroup = new SavedGroupModel();
            if (!String.IsNullOrEmpty(ChatId))
                GetChat();

            GetUsers();
        }

        public override void OnLostSelection()
        {
            UserList?.Clear();
            AvaibleUserList?.Clear();
            ChatId = null;
            ChatName = null;
            IsPublic = false;
            CreatorMode = false;
            PhotoPath = null;
            savedGroup = null;
            _errors.Clear();
        }
    }
}
