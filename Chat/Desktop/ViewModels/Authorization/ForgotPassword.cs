using ChatCore.Factories;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using ChatDesktop.ViewModels.Dialogs;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ChatDesktop.ViewModels.Authorization
{
    public class ForgotPassword : AuthBase
    {
        public ForgotPassword(IAccount account, IChatHub chat, IAbstractFactory<Window> abstractFactory) : base(account, chat, abstractFactory)
        {
        }

        private ICommand _reset;
        public ICommand ResetCommand => _reset == null ? _reset = new RelayCommand(ResetPassword) : _reset;

        private async void ResetPassword(object obj)
        {
            if (String.IsNullOrEmpty(UserName) || !IsValid)
                return;

            var result = await _account.ResetPassword(UserName);

            if (result)
            {
                var message = new BaseDialogVM(Resources.Lang.Localization.Instance["SendPasswordEmail"]);
                //await DialogHost.Show(message);
            }
        }
    }
}
