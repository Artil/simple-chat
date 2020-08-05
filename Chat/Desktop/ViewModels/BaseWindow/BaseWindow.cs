using ChatCore.Models;
using ChatDesktop.Models;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Input;

namespace ChatDesktop.ViewModels.BaseWindow
{
    public class BaseWindow : ViewModelBase
    {
        private ICommand _closeWindow;
        public ICommand CloseWindowCommand => _closeWindow == null ? _closeWindow = new RelayCommand(CloseWindowAction) : _closeWindow;

        private ICommand _hideWindow;
        public ICommand HideWindowCommand => _hideWindow == null ? _hideWindow = new RelayCommand(HideWindowAction) : _hideWindow;

        private ICommand _minMaxWindow;
        public ICommand MinMaxWindowCommand => _minMaxWindow == null ? _minMaxWindow = new RelayCommand(MinMaxWindowAction) : _minMaxWindow;

        private void CloseWindowAction(object obj)
        {
            Properties.UserSettings.Default.Notifications = JsonConvert.SerializeObject(Client.Notifications);
            Properties.UserSettings.Default.Save();
            Application.Current.Shutdown();
        }

        private void HideWindowAction(object obj)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MinMaxWindowAction(object obj)
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            else
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }
    }
}
