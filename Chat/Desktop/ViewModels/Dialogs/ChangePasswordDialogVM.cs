using ChatCore.Languages;
using ChatDesktop.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ChatDesktop.ViewModels.Dialogs
{
    public class ChangePasswordDialogVM : BaseDialogVM
    {
        public ChangePasswordDialogVM(string text) : base(text)
        {
        }

        private string _password;
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

        private string _newPassword;
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(lang))]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*,.-]).*$", ErrorMessageResourceName = "PasswordError", ErrorMessageResourceType = typeof(lang))]
        [MinLength(7, ErrorMessageResourceName = "MinPasswordLength", ErrorMessageResourceType = typeof(lang))]
        [NotEqualValidation("Password", ErrorMessageResourceName = "IdenticalPasswords", ErrorMessageResourceType = typeof(lang))]
        [DataType(DataType.Password)]
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (_newPassword == value)
                    return;

                _newPassword = value;
                ValidateProperty(value);
                OnPropertyChanged();
            }
        }
    }
}
