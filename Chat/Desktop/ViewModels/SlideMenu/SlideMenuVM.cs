using ChatCore.Models;
using ChatDesktop.Enums;
using ChatDesktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatDesktop.ViewModels.SlideMenu
{
    public class SlideMenuVM : ViewModelBase
    {
        public SlideMenuVM(IEnumerable<SlideMenuBase> tabs)
        {
            Tabs = new ObservableCollection<SlideMenuBase>(tabs);
            currentViewModel = Tabs.Skip(1).First();
        }

        public ObservableCollection<SlideMenuBase> Tabs;

        private SlideMenuBase currentViewModel;

        public SlideMenuBase CurrentViewModel
        {
            get => currentViewModel;
            set
            {
                if (currentViewModel.Equals(value))
                    return;
                currentViewModel = value;
                OnPropertyChanged();
            }
        }

        private string _curentTabName = "chat";
        public string CurrentTabName
        {
            get => _curentTabName;
            set
            {
                if (String.Equals(_curentTabName, value))
                    return;

                _curentTabName = value;
                OnPropertyChanged();
            }
        }

        public string Name { get => Client.CurrentUser?.Name; }
        public string PhotoPath { get => Client.CurrentUser?.PhotoPath; }
        public string Color { get => Client.CurrentUser?.Color; }
        public string ShortName { get => Client.CurrentUser?.ShortName; }

        private ICommand _openMenu;
        public ICommand OpenMenuCommand => _openMenu == null ? _openMenu = new RelayCommand(OpenMenuAction) : _openMenu;

        private ICommand _closeMenu;
        public ICommand CloseMenuCommand => _closeMenu == null ? _closeMenu = new RelayCommand(CloseMenuAction) : _closeMenu;

        private ICommand _switchTabs;
        public ICommand SwitchTabsCommand => _switchTabs == null ? _switchTabs = new RelayCommand(SwitchTabsAction) : _switchTabs;

        private ICommand _goToUserView;
        public ICommand GoToUserViewCommand => _goToUserView == null ? _goToUserView = new RelayCommand(GoToUserView) : _goToUserView;

        private ICommand _goToSettingsView;
        public ICommand GoToSettingsViewCommand => _goToSettingsView == null ? _goToSettingsView = new RelayCommand(GoToSettingsView) : _goToSettingsView;

        private void SwitchTabsAction(object obj)
        {
            if (ReferenceEquals(obj, null))
                return;

            var newTab = obj as Button;
            CurrentTabName = newTab.Name;

            switch (newTab.Name) 
            {
                case "addUser":
                    CurrentViewModel = Tabs.First();
                    break;
                case "chat":
                    CurrentViewModel = Tabs.Skip(1).First();
                    break;
                case "contacts":
                    CurrentViewModel = Tabs.Last();
                    break;
                default:
                    break;
            }
        }


        private void OpenMenuAction(object obj)
        {
            var values = (object[])obj;
            var ButtonOpenMenu = values[0] as Button;
            var ButtonCloseMenu = values[1] as Button;
            var UserPanel = values[2] as Grid;

            (CurrentViewModel as ChatListVM).ScrollVisible = false;

            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            UserPanel.Visibility = Visibility.Visible;
        }

        private void CloseMenuAction(object obj)
        {
            var values = (object[])obj;
            var ButtonOpenMenu = values[0] as Button;
            var ButtonCloseMenu = values[1] as Button;
            var UserPanel = values[2] as Grid;

            CurrentViewModel = Tabs.Skip(1).FirstOrDefault();

            (CurrentViewModel as ChatListVM).ScrollVisible = true;

            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
            UserPanel.Visibility = Visibility.Collapsed;

            CurrentTabName = "chat";
        }

        private void GoToUserView(object obj)
        {
            var user = new { UserName = obj as String };
            Client.ChangeBaseContent(BaseContentEnum.UserView, user);
        }

        private void GoToSettingsView(object obj)
        {
            Client.ChangeBaseContent(BaseContentEnum.AppSettings, null);
        }

        public void GetCurrentUser()
        {
            OnPropertyChanged("Name");
            OnPropertyChanged("PhotoPath");
            OnPropertyChanged("ShortName");
            OnPropertyChanged("Color");

        }
    }
}