using ChatCore.Enums;
using ChatCore.Factories;
using ChatCore.Languages;
using ChatCore.Models;
using ChatCore.Services;
using ChatDesktop.Attributes;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Input;

namespace ChatDesktop.ViewModels.Authorization
{
    public class RegisterVM : AuthBase
    {
        public RegisterVM(IAccount account, IChatHub chat, IAbstractFactory<Window> abstractFactory) : base(account, chat, abstractFactory)
        {

        }

        ICommand registerCommand;
        public ICommand RegisterCommand => registerCommand == null ? registerCommand = new RelayCommand(RegisterCommandAction) : registerCommand;

        private string _email;
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(lang))]
        [EmailAddress(ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(lang))]
        public string Email
        {
            get => _email;
            set
            {
                if (_email == value)
                    return;

                _email = value;
                ValidateProperty(value);
                OnPropertyChanged();
            }
        }

        private string _confirmPassword;
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(lang))]
        [EqualsValidation("Password", ErrorMessageResourceName = "PasswordsNotIdentical", ErrorMessageResourceType = typeof(lang))]
        [DataType(DataType.Password)]
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (_confirmPassword == value)
                    return;

                _confirmPassword = value;
                ValidateProperty(value);
                OnPropertyChanged();
            }
        }

        public async void RegisterCommandAction(object obj) 
        {
            if (String.IsNullOrEmpty(UserName) ||
                String.IsNullOrEmpty(Email) ||
                String.IsNullOrEmpty(Password) ||
                String.IsNullOrEmpty(ConfirmPassword) ||
                !IsValid)
                return;

            IsLoading = true;

            try
            {
                await _account.SwapKeys(Rsa.PublicKey);
                var rm = new RegisterModel { UserName = UserName, Email = _email, Password = Password, PasswordConfirm = _confirmPassword };

                var result = await _account.Register(JsonConvert.SerializeObject(rm));

                if (result.Equals(AuthorizeResultEnum.Ok))
                    StartService(obj);
                else
                {
                    switch (result)
                    {
                        case AuthorizeResultEnum.EmailExist:
                            ErrorText = Resources.Lang.Localization.Instance["EmailExist"];
                            break;

                        case AuthorizeResultEnum.UserNameExist:
                            ErrorText = Resources.Lang.Localization.Instance["UserNameExist"];
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

        public override void Clear()
        {
            Email = String.Empty;
            ConfirmPassword = String.Empty;
            base.Clear();
        }
    }
}
