using ChatCore.Factories;
using ChatCore.Languages;
using ChatCore.Services;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using ChatDesktop.Resources.Lang;
using ChatDesktop.Views.BaseWindow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace ChatDesktop.ViewModels.Authorization
{
    public class AuthBase : DataErrorInfoVM
    {
        protected readonly IAccount _account;
        protected readonly IChatHub _chat;
        protected readonly Window _mainWindow;

        public AuthBase(IAccount account, IChatHub chat, IAbstractFactory<Window> abstractFactory)
        {
            _account = account;
            _chat = chat;
            _mainWindow = abstractFactory.GetInstance();
        }

        private string _userName;
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(lang))]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessageResourceName = "UsernameError", ErrorMessageResourceType = typeof(lang))]
        [StringLength(12, ErrorMessageResourceName = "MinUsernameLength", ErrorMessageResourceType = typeof(lang), MinimumLength = 4)]
        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName == value)
                    return;

                _userName = value;
                ValidateProperty(value);
                OnPropertyChanged();
            }
        }

        protected string _password;
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(lang))]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*,.-]).*$", ErrorMessageResourceName = "PasswordError", ErrorMessageResourceType = typeof(lang))]
        [MinLength(7, ErrorMessageResourceName = "MinPasswordLength", ErrorMessageResourceType = typeof(lang))]
        [DataType(DataType.Password)]
        public string Password
        {
            get => _password; 
            set
            {
                if (_password == value)
                    return;

                _password = value;
                ValidateProperty(value);
                OnPropertyChanged();
            }
        }

        private string _errorText;
        public string ErrorText
        {
            get => _errorText;
            set
            {
                if (String.Equals(_errorText, value))
                    return;

                _errorText = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading.Equals(value))
                    return;

                _isLoading = value;

                //if (value)
                //    Task.Run(() => LoadAnimation());
                //else
                //    LoadIter = 100;

                OnPropertyChanged();
            }
        }

        //private int _loadIter;
        //public int LoadIter
        //{
        //    get => _loadIter;
        //    set
        //    {
        //        if (_loadIter == value)
        //            return;

        //        _loadIter = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private void LoadAnimation()
        //{
        //    for (int LoadIter = 0; LoadIter < 100; LoadIter +=11)
        //    {
        //        if (LoadIter == 99)
        //            LoadIter = 0;
        //    }
        //}

        public virtual void Clear() 
        {
            UserName = String.Empty;
            Password = String.Empty;
            ErrorText = String.Empty;
            _errors.Clear();
        }

        protected void StartService(object obj)
        {
            _chat.ChatHubManager();

            var authorizeWindow = obj as AuthorizeWindow;
            Application.Current.MainWindow = _mainWindow;
            Client.Notifications = JsonConvert.DeserializeObject<List<string>>(Properties.UserSettings.Default.Notifications) ?? new List<string>();
            _mainWindow.Show();
            IsLoading = false;
            authorizeWindow.Close();
        }
    }
}
