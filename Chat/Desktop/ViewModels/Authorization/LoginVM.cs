using ChatCore.Enums;
using ChatCore.Factories;
using ChatCore.Models;
using ChatCore.Services;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using ChatDesktop.Views.BaseWindow;
using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Input;

namespace ChatDesktop.ViewModels.Authorization
{
    public class LoginVM : AuthBase
    {
        public LoginVM(IAccount account, IChatHub chat, IAbstractFactory<Window> abstractFactory) : base(account, chat, abstractFactory)
        {
            UserName = Properties.UserSettings.Default.Login;
            Password = Properties.UserSettings.Default.Password;
            RememberMe = Properties.UserSettings.Default.RememberMe;
            _errors.Clear();
        }

        private ICommand _login;
        public ICommand LoginCommand => _login == null ? _login = new RelayCommand(LoginAction) : _login;

        private bool _rememberMe;
        public bool RememberMe
        {
            get => _rememberMe;
            set
            {
                if (_rememberMe == value)
                    return;
                _rememberMe = value;
                OnPropertyChanged();
            }
        }

        public async void LoginAction(object obj)
        {
            if (String.IsNullOrEmpty(UserName) ||
                String.IsNullOrEmpty(Password) ||
                !IsValid)
                return;

            IsLoading = true;

            try
            {
                await _account.SwapKeys(Rsa.PublicKey);
                var lm = new LoginModel { UserName = UserName, Password = Password, RememberMe = _rememberMe };

                var result = await _account.Login(JsonConvert.SerializeObject(lm));

                if (result.Equals(AuthorizeResultEnum.Ok))
                {
                    if (RememberMe)
                    {
                        Properties.UserSettings.Default.Login = UserName;
                        Properties.UserSettings.Default.Password = Password;
                        Properties.UserSettings.Default.RememberMe = RememberMe;
                        Properties.UserSettings.Default.Save();
                    }
                    else
                    {
                        Properties.UserSettings.Default.Login = String.Empty;
                        Properties.UserSettings.Default.Password = String.Empty;
                        Properties.UserSettings.Default.RememberMe = RememberMe;
                        Properties.UserSettings.Default.Save();
                    }

                    StartService(obj);
                }
                else
                {
                    switch (result)
                    {
                        case AuthorizeResultEnum.UserNotFound:
                            ErrorText = Resources.Lang.Localization.Instance["UserNotFound"];
                            break;

                        case AuthorizeResultEnum.WrongLoginOrPassword:
                            ErrorText = Resources.Lang.Localization.Instance["WrongLoginOrPassword"];
                            break;

                        default:
                            break;
                    }
                }
                IsLoading = false;
            }
            catch
            {
                IsLoading = false;
                ErrorText = Resources.Lang.Localization.Instance["ServerIsNotResponding"];
            }
        }
    }

}
