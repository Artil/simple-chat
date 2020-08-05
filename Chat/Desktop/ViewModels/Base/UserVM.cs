using ChatCore.Models;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using ChatDesktop.ViewModels.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace ChatDesktop.ViewModels.Base
{
    public class UserVM : BaseContentVM
    {
        private readonly IChatHub _chat;
        private readonly IClone<UserModel> _clone;
        private readonly IFile _file;
        public event Action CurrentUserLoaded;
        public UserVM(IChatHub chat, IClone<UserModel> clone, IFile file)
        {
            _chat = chat;
            _clone = clone;
            _file = file;
            _chat.ConnectionStarted += GetCurrentUser;
        }

        private ICommand _lock;
        public ICommand LockCommand => _lock == null ? _lock = new RelayCommand(LockAction) : _lock;

        private ICommand _removeChanges;
        public ICommand RemoveChangesCommand => _removeChanges == null ? _removeChanges = new RelayCommand(RemoveChangesAction) : _removeChanges;

        private ICommand _saveChanges;
        public ICommand SaveChangesCommand => _saveChanges == null ? _saveChanges = new RelayCommand(SaveChangesAction) : _saveChanges;

        private ICommand _changeImage;
        public ICommand ChangeImageCommand => _changeImage == null ? _changeImage = new RelayCommand(ChangeImageAction) : _changeImage;

        private ICommand _changePassword;
        public ICommand ChangePasswordCommand => _changePassword == null ? _changePassword = new RelayCommand(ChangePasswordAction) : _changePassword;

        private UserModel _user;
        public UserModel User
        {
            get => _user;
            set
            {
                if (ReferenceEquals(_user, value))
                    return;

                _user = value;
                OnPropertyChanged();
            }
        }

        private string _userName = null;
        public string UserName
        {
            get => _userName;
            set
            {
                if (String.Equals(_userName, value))
                    return;

                _userName = value;
                OnPropertyChanged();
            }
        }

        private bool _canChange;
        public bool CanChange
        {
            get => _canChange;
            set
            {
                if (_canChange.Equals(value))
                    return;

                _canChange = value;
                OnPropertyChanged();
            }
        }

        private bool _isLocked;
        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (_isLocked.Equals(value))
                    return;

                _isLocked = value;
                OnPropertyChanged();
            }
        }

        private FileModel _fileModel;
        public FileModel FileModel
        {
            get => _fileModel;
            set
            {
                _fileModel = value;
                OnPropertyChanged();
            }
        }

        private async void GetCurrentUser()
        {
            CanChange = true;
            if (ReferenceEquals(Client.CurrentUser, null))
            {
                var user = await _chat.GetUser(UserName);
                await _file.GetPhotos(new[] { user });
                User = user;
                Client.CurrentUser = _clone.Clone(User);
                _chat.ConnectionStarted -= GetCurrentUser;
                CurrentUserLoaded.Invoke();
            }
            else
                User = _clone.Clone(Client.CurrentUser);
        }

        private async void GetUser()
        {
            CanChange = false;
            var user = await _chat.GetUser(UserName);
            await _file.GetPhotos(new[] { user });
            User = user;
        }

        private Flipper Flipper;
        private void LockAction(object obj)
        {
            Flipper = obj as Flipper;
            Flipper.IsFlipped = !Flipper.IsFlipped;
            IsLocked = Flipper.IsFlipped;
        }

        private void RemoveChangesAction(object obj)
        {
            User = _clone.Clone(Client.CurrentUser);
        }

        private async void SaveChangesAction(object obj)
        {
            if (!ReferenceEquals(FileModel, null))
            {
                var url = $"/SendPhoto?userId={User.Id}";
                var result = await _file.SendFileToApi(url, FileModel);
                result = await _file.GetFile(result);

                User.PhotoPath = result;

                if (Client.UsersMedia.Any(x => x.Key == User.Name))
                    Client.UsersMedia.Remove(User.Name);

                Client.GetSavedMedia(new[] { User });
                FileModel = null;
            }

            await _chat.UpdateUserInfo(User);
            Client.CurrentUser = _clone.Clone(User);
        }

        private async void ChangeImageAction(object obj)
        {
            var openFileDialog = new CommonOpenFileDialog();

            openFileDialog.Filters.Add(new CommonFileDialogFilter("All Images Files", "*.png;*.jpeg;*.jpg;*.bmp;*.tiff;*.tif"));
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FileModel = new FileModel()
                {
                    FileFullName = Path.GetFileName(openFileDialog.FileName),
                    FileContent = File.ReadAllBytes(openFileDialog.FileName),
                };
                User.PhotoPath = openFileDialog.FileName;
            }
        }

        private async void ChangePasswordAction(object obj)
        {
            var dialog = new ChangePasswordDialogVM(Resources.Lang.Localization.Instance["ChangePassword"]);
            var result = await DialogHost.Show(dialog);

            if (ReferenceEquals(result, null) || result.Equals(false) || !dialog.IsValid)
                return;

        }

        public override void OnLostSelection()
        {
            if (!ReferenceEquals(Flipper, null))
            {
                Flipper.IsFlipped = false;
                Flipper.OnApplyTemplate();
                Flipper = null;
            }
        }

        public override void OnSelected()
        {
            if (String.Equals(UserName, Client.CurrentUser.Name))
                GetCurrentUser();
            else
                GetUser();
        }
    }
}
