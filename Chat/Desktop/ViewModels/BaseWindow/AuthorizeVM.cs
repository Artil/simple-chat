using ChatCore.Models;
using ChatDesktop.Models;
using ChatDesktop.Resources.Lang;
using ChatDesktop.ViewModels.Authorization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ChatDesktop.ViewModels.BaseWindow
{
    public class AuthorizeVM : BaseWindow
    {
        public AuthorizeVM(IEnumerable<AuthBase> tabs)
        {
            Tabs = new ObservableCollection<AuthBase>(tabs);
            _currentViewModel = Tabs.First();
        }

        public ObservableCollection<AuthBase> Tabs;

        private AuthBase _currentViewModel;

        public AuthBase CurrentViewModel
        {
            get => _currentViewModel;

            set
            {
                if (_currentViewModel.Equals(value))
                    return;

                _currentViewModel.Clear();
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }
        private string _text = Localization.Instance["AccountIsntExist"];

        public string text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        private string _link = Localization.Instance["CreateAccount"];
        public string link
        {
            get => _link; 
            set
            {
                _link = value;
                OnPropertyChanged();
            }
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled.Equals(value))
                    return;

                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        ICommand authorizeCommand;
        public ICommand AuthorizeCommand => authorizeCommand == null ? authorizeCommand = new RelayCommand(AuthorizeCommandAction) : authorizeCommand;

        private void AuthorizeCommandAction(object obj)
        {
            if (!(obj is null))
            {
                text = Localization.Instance["RememberPassword"];
                link = Localization.Instance["Login"];
                IsEnabled = false;
                CurrentViewModel = Tabs.Last();
                return;
            }
            if (CurrentViewModel.GetType() == typeof(LoginVM))
            {
                text = Localization.Instance["AccountExist"];
                link = Localization.Instance["Login"];
                IsEnabled = false;
                CurrentViewModel = Tabs.Skip(1).First();
            }
            else
            {
                text = Localization.Instance["AccountIsntExist"];
                link = Localization.Instance["CreateAccount"];
                IsEnabled = true;
                CurrentViewModel = Tabs.First();
            }
        }
    }
}
