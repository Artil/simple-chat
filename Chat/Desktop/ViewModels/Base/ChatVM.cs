using ChatCore.Enums;
using ChatCore.Models;
using ChatDesktop.Enums;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using ChatDesktop.ViewModels.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ChatDesktop.ViewModels.Base
{
    public class ChatVM : BaseContentVM, IFileDragDropTarget
    {
        private readonly IChatHub _chat;
        private readonly IFile _file;
        private readonly IConference _conference;
        public event Action<MessageModel> NewMyMessage;
        public event Action<MessageModel> RemoveMyMessage;
        public event Action<string> NewChatCreated;

        private Timer timer;
        public ChatVM(IChatHub chat, IFile file, IConference conference)
        {
            _chat = chat;
            _file = file;
            _conference = conference;
            _chat.NewCompanionMessage += AddMessage;
            _chat.MessageDeleted += DeleteMessage;
            _chat.MessageUpdated += UpdateMessage;
        }

        private ICommand _sendMessage;
        public ICommand SendMessageCommand => _sendMessage == null ? _sendMessage = new RelayCommand(SendMessageAction) : _sendMessage;

        private ICommand _attachFile;
        public ICommand AttachFileCommand => _attachFile == null ? _attachFile = new RelayCommand(AttachFileAction) : _attachFile;

        private ICommand _removeAttachedFile;
        public ICommand RemoveAttachedFileCommand => _removeAttachedFile == null ? _removeAttachedFile = new RelayCommand(RemoveAttachedFileAction) : _removeAttachedFile;

        private ICommand _removeMessage;
        public ICommand RemoveMessageCommand => _removeMessage == null ? _removeMessage = new RelayCommand(RemoveMessageAction) : _removeMessage;

        private ICommand _tryUpdateMessage;
        public ICommand TryUpdateMessageCommand => _tryUpdateMessage == null ? _tryUpdateMessage = new RelayCommand(TryUpdateMessageAction) : _tryUpdateMessage;

        private ICommand _cancelUpdateMessage;
        public ICommand CancelUpdateMessageCommand => _cancelUpdateMessage == null ? _cancelUpdateMessage = new RelayCommand(CancelMessageAction) : _cancelUpdateMessage;

        private ICommand _updateMessage;
        public ICommand UpdateMessageCommand => _updateMessage == null ? _updateMessage = new RelayCommand(UpdateMessageAction) : _updateMessage;

        private ICommand _downloadFile;
        public ICommand DownloadFileCommand => _downloadFile == null ? _downloadFile = new RelayCommand(DownloadFileAction) : _downloadFile;

        private ICommand _forwardMessage;
        public ICommand ForwardMessageCommand => _forwardMessage == null ? _forwardMessage = new RelayCommand(ForwardMessageAction) : _forwardMessage;

        private ICommand _changeBaseContent;
        public ICommand ChangeBaseContentCommand => _changeBaseContent == null ? _changeBaseContent = new RelayCommand(ChangeBaseContentAction) : _changeBaseContent;

        private ICommand _copy;
        public ICommand CopyCommand => _copy == null ? _copy = new RelayCommand(CopyAction) : _copy;

        private ICommand _openFile;
        public ICommand OpenFileCommand => _openFile == null ? _openFile = new RelayCommand(OpenFileAction) : _openFile;

        private ICommand _getMessages;
        public ICommand GetMessagesCommand => _getMessages == null ? _getMessages = new RelayCommand(GetMessages) : _getMessages;

        private ICommand _stopNotifications;
        public ICommand StopNotificationsCommand => _stopNotifications == null ? _stopNotifications = new RelayCommand(StopNotifications) : _stopNotifications;

        private ICommand _captureVoice;
        public ICommand CaptureVoiceCommand => _captureVoice == null ? _captureVoice = new RelayCommand(CaptureVoice) : _captureVoice;

        private ICommand _captureVideo;
        public ICommand CaptureVideoCommand => _captureVideo == null ? _captureVideo = new RelayCommand(CaptureVideo) : _captureVideo;

        private string _chatName;
        public string ChatName
        {
            get => _chatName;
            set
            {
                if (String.Equals(_chatName, value))
                    return;

                _chatName = value;
                isChatChanged = true;
                OnPropertyChanged("ShortName");
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

        public string ShortName => ChatName.Substring(0, 1).ToUpper();

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

        private ChatEnum _chatEnum;
        public ChatEnum ChatEnum
        {
            get => _chatEnum;
            set
            {
                if (_chatEnum.Equals(value))
                    return;

                _chatEnum = value;
                if (_chatEnum.Equals(ChatEnum.ChatForGroup))
                    IsChatForGroup = true;
                else
                    IsChatForGroup = false;
                OnPropertyChanged();
            }
        }

        private bool _isChatForGroup;
        public bool IsChatForGroup
        {
            get => _isChatForGroup;
            set
            {
                if (_isChatForGroup.Equals(value))
                    return;

                _isChatForGroup = value;
                OnPropertyChanged();
            }
        }

        private bool _isUserOnline;
        public bool IsUserOnline
        {
            get => _isUserOnline;
            set
            {
                if (_isUserOnline.Equals(value))
                    return;

                _isUserOnline = value;
                OnPropertyChanged();
            }
        }

        private bool _isSendAvaible;
        public bool IsSendAvaible
        {
            get => _isSendAvaible;
            set
            {
                if (_isSendAvaible.Equals(value))
                    return;

                _isSendAvaible = value;
                OnPropertyChanged();
            }
        }

        private bool _isCapture;
        public bool IsCapture
        {
            get => _isCapture;
            set
            {
                if (_isCapture.Equals(value))
                    return;

                _isCapture = value;
                OnPropertyChanged();
            }
        }

        private bool _isVideoCapture;
        public bool IsVideoCapture
        {
            get => _isVideoCapture;
            set
            {
                if (_isVideoCapture.Equals(value))
                    return;

                _isVideoCapture = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MessageModel> _messages = new ObservableCollection<MessageModel>();
        public ObservableCollection<MessageModel> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        private string _content = String.Empty;
        public string Content
        {
            get => _content;
            set
            {
                if (ReferenceEquals(_content, value))
                    return;

                if (String.IsNullOrEmpty(value))
                    IsSendAvaible = false;
                else
                    IsSendAvaible = true;

                _content = value;
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

        private bool _isUpdateMessage;
        public bool IsUpdateMessage
        {
            get => _isUpdateMessage;
            set
            {
                if (_isUpdateMessage.Equals(value))
                    return;

                _isUpdateMessage = value;
                OnPropertyChanged();
            }
        }

        private bool _notification;
        public bool Notification
        {
            get => _notification;
            set
            {
                if (_notification.Equals(value))
                    return;

                _notification = value;
                OnPropertyChanged();
            }
        }

        private string IdForUpdate { get; set; }

        private async void GetMessages(object obj)
        {
            if (Messages.Any(x => x.IsLast))
                return;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
            {
                var result = await _chat.GetMessages(ChatId, Messages.Count);
                if (result is null)
                    return;
                if (!String.Equals(result.FirstOrDefault()?.ChatId, ChatId))
                    return;

                foreach (var mes in result.Where(x => !ReferenceEquals(x.File, null)))
                {
                    mes.File.TempFilePath = await _file.GetFile(mes.File.FileFullName);
                }

                await _file.GetPhotos(result);

                foreach (var item in result)
                {
                    Messages.Insert(0, item);
                }
            }));
        }

        private void SetTimer()
        {
            var tm = new TimerCallback(IsUserOnlineCheck); // set callback
            timer = new Timer(tm, 0, 0, 1000 * 60 * 5); // 5 min
        }

        private async void IsUserOnlineCheck(object obj)
        {
           // IsUserOnline = await _chat.IsUserOnlineCheck(ChatName);
        }

        public async void SendMessageAction(object obj)
        {
            if ( (String.IsNullOrEmpty(Content) || String.IsNullOrWhiteSpace(Content)) && ReferenceEquals(FileContent, null))
                return;

            try
            {
                string result = null;
                if (!ReferenceEquals(FileContent, null))
                {
                    var url = $"/SendFile?chatId={ChatId}";
                    result = await _file.SendFileToApi(url, FileContent);
                }

                var message = new MessageModel()
                {
                    ChatId = ChatId,
                    ToChatName = ChatName,
                    Content = String.IsNullOrEmpty(result) ? Content : Path.GetFileName(result),
                    File = String.IsNullOrEmpty(result) ? null : new FileModel() { FileFullName = result, IsImage = FileContent.IsImage, IsVoice = FileContent.IsVoice, IsVideo = FileContent.IsVideo },
                    Name = Client.CurrentUser.Name,
                    IsMyMessage = true
                };

                message = await _chat.SendMessage(message);

                if (String.IsNullOrEmpty(ChatId))
                    NewChatCreated?.Invoke(ChatEnum.Equals(ChatEnum.ChatForGroup) ? ChatId : ChatName);

                ChatId = message.ChatId;
                AddMessage(message);
                Content = String.Empty;
                FileContent = null;
                IsSendAvaible = false;
            }
            catch { }
        }

        public void AttachFileAction(object obj)
        {
            if (String.IsNullOrEmpty(ChatId))
                return;
            
            var openFileDialog = new CommonOpenFileDialog();

            if ((bool)obj)
                openFileDialog.Filters.Add(new CommonFileDialogFilter("All Images Files", "*.png;*.jpeg;*.jpg;*.bmp;*.tiff;*.tif"));

            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FileContent = new FileModel()
                {
                    FileFullName = Guid.NewGuid().ToString() + Path.GetExtension(openFileDialog.FileName),
                    FileContent = File.ReadAllBytes(openFileDialog.FileName),
                    IsImage = (bool)obj
                };
                IsSendAvaible = true;
            }
        }

        public void RemoveAttachedFileAction(object obj)
        {
            FileContent = null;
            IsSendAvaible = false; 
        }

        public async void RemoveMessageAction(object obj) 
        {
            var dialog = new RemoveMessageDialogVM(Resources.Lang.Localization.Instance["RemoveQuestion"]);
            var result = await DialogHost.Show(dialog);

            if (ReferenceEquals(result, null) || result.Equals(false))
                return;

            var message = obj as MessageModel;
            RemoveMyMessage?.Invoke(message);
            await _chat.DeleteMessage(message, dialog.RemoveType);
            DeleteMessage(message);
        }

        public void TryUpdateMessageAction(object obj)
        {
            var message = obj as MessageModel;
            IsUpdateMessage = true;
            IdForUpdate = message.MessageId;
            Content = message.Content;
        }

        public void CancelMessageAction(object obj)
        {
            IsUpdateMessage = false;
            IdForUpdate = String.Empty;
            Content = String.Empty;
        }

        public async void UpdateMessageAction(object obj)
        {
            if (String.IsNullOrEmpty(IdForUpdate))
                return;

            var message = Messages.FirstOrDefault(x => String.Equals(x.MessageId, IdForUpdate));
            message.Content = Content;
            var updatedMessage = await _chat.UpdateMessage(message);

            UpdateMessage(updatedMessage);
            Content = String.Empty;
            IsUpdateMessage = false;
        }

        private void DeleteMessage(MessageModel message)
        {
            try
            {
                if(String.IsNullOrEmpty(message.ForwardMessageId))
                    Messages.Remove(Messages.FirstOrDefault(x => String.Equals(x.MessageId, message.MessageId)));
                else
                    Messages.Remove(Messages.FirstOrDefault(x => String.Equals(x.ForwardMessageId, message.ForwardMessageId)));
            }
            catch { }
        }

        private async void DownloadFileAction(object obj)
        {
            var file = Messages.FirstOrDefault(x => Object.Equals(x.File, obj as FileModel)).File;
            await _file.GetFileWithProgress(file);
        }

        private async void ForwardMessageAction(object obj)
        {
            if (ReferenceEquals(obj, null))
                return;

            var dialog = new ForwardMessageDialogVM(_chat, Resources.Lang.Localization.Instance["ChooseChat"], obj.ToString());
            dialog.PostForwardMessage += AddMessage;
            await DialogHost.Show(dialog);
            dialog.PostForwardMessage -= AddMessage;
        }

        private void UpdateMessage(MessageModel updatedMessage)
        {
            var message = Messages.FirstOrDefault(x => String.Equals(x.MessageId, updatedMessage.MessageId));
            message.Content = updatedMessage.Content;
            message.UpdateTime = updatedMessage.UpdateTime;
        }

        private async void AddMessage(MessageModel message)
        {
            if(message.ChatId == ChatId)
            {
                if(!ReferenceEquals(message.File, null))
                    message.File.TempFilePath = await _file.GetFile(message.File.FileFullName);

                await _file.GetPhotos(new[] { message });

                Messages.Add(message);
                await _chat.ReadMessages(ChatId);
            }

            if (message.IsMyMessage == true)
                NewMyMessage?.Invoke(message);
        }

        private void ChangeBaseContentAction(object obj)
        {
            if (obj is null)
                return;

            if (IsChatForGroup && String.Equals(obj.ToString(), ChatName))
            {
                GoToGroupSettings();
                return;
            }

            var user = new { UserName = obj as String };

            Client.ChangeBaseContent(BaseContentEnum.UserView, user);
        }

        private void GoToGroupSettings()
        {
            var group = new { ChatId, ChatName, Color, PhotoPath };
            Client.ChangeBaseContent(BaseContentEnum.GroupCreateView, group);
        }

        private void CopyAction(object obj)
        {
            var message = Messages.FirstOrDefault(x => String.Equals(x.MessageId, obj.ToString()));
            Clipboard.SetDataObject(message.Content);
        }

        private async void OpenFileAction(object obj)
        {
            //var file = obj as FileModel;
            //var fileDirectory = "AppFiles/TempFiles/";

            //if (ReferenceEquals(file, null))
            //    return;

            //if (!Directory.Exists(fileDirectory))
            //    Directory.CreateDirectory(fileDirectory);

            //file.TempFilePath = fileDirectory + file.FileName;

            //await File.WriteAllBytesAsync(file.TempFilePath, file.FileContent);

        }

        private void StopNotifications(object obj)
        {
            if (String.IsNullOrEmpty(ChatId))
                return;

            if (!Notification)
                Client.Notifications.Add(ChatId);
            else
                Client.Notifications.Remove(ChatId);
        }

        private void CaptureVoice(object obj)
        {
            if (!IsCapture)
            {
                _conference.StartCapture(Guid.NewGuid().ToString() + ".wav");
                IsCapture = true;
                _conference.Stop += CaptureStopped;
            }
            else
            {
                _conference.StopAllCapture();
            }
        }

        private void CaptureVideo(object obj)
        {
            if (!IsCapture)
            {
                _conference.StartCapture(Guid.NewGuid().ToString() + ".avi", CaptureTypeEnum.VideoCaptureWithVoice);
                IsCapture = true;
                _conference.Stop += CaptureStopped;
            }
            else
            {
                _conference.StopAllCapture();
            }
        }

        private void CaptureStopped(string fileName)
        {
            IsCapture = false;

            if(!String.IsNullOrEmpty(fileName))
                AttachMediaMessage(fileName);
        }

        private async void AttachMediaMessage(string fileName)
        {
            var file = await _file.GetFile(fileName);

            if (file is null)
                return;

            FileContent = new FileModel()
            {
                FileFullName = Path.GetFileName(file),
                FileContent = File.ReadAllBytes(file),
                IsVoice = !IsVideoCapture,
                IsVideo = IsVideoCapture
            };
            IsSendAvaible = true;
        }

        public override void OnSelected()
        {
            _chat.ReadMessages(ChatId); // read messages
            SetTimer(); // set new timer

            if (!isChatChanged)
                return;

            Notification = !Client.Notifications.Any(x => String.Equals(ChatId, x));

            Messages.Clear();

            if (!String.IsNullOrEmpty(ChatId)) // get messages
                GetMessages(null);

            isChatChanged = false;
        }

        private bool isChatChanged;
        public override void OnLostSelection()
        {
            if (!ReferenceEquals(timer, null)) // remove time if exist
                timer.Dispose();
        }

        public void OnFileDrop(string[] filepaths)
        {
            FileContent = new FileModel()
            {
                FileFullName = Guid.NewGuid().ToString() + Path.GetExtension(filepaths[0]),
                FileContent = File.ReadAllBytes(filepaths[0]),
            };
            IsSendAvaible = true;
        }
    }
}
