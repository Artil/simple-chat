using Accord.DirectSound;
using Accord.Video.DirectShow;
using ChatDesktop.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ChatDesktop.ViewModels.Base
{
    public class AppSettingsVM : BaseContentVM
    {
        public AppSettingsVM()
        {
            SelectedLang = Properties.UserSettings.Default.Lang;
            AllNotifications = Properties.UserSettings.Default.AllNotifications;
        }

        private ObservableCollection<FilterInfo> _videoDevices = new ObservableCollection<FilterInfo>();
        public ObservableCollection<FilterInfo> VideoDevices
        {
            get => _videoDevices;
            set
            {
                _videoDevices = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AudioDeviceInfo> _audioDevices = new ObservableCollection<AudioDeviceInfo>();
        public ObservableCollection<AudioDeviceInfo> AudioDevices
        {
            get => _audioDevices;
            set
            {
                _audioDevices = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, string> _langs = new Dictionary<string, string>
        {
            {"English", "" },
            {"Українська", "uk-UA" },
            {"Русский", "ru-RU" }
        };

        public Dictionary<string, string> Langs
        {
            get => _langs;
            set
            {
                _langs = value;
                OnPropertyChanged();
            }
        }

        private string _selectedLang;
        public string SelectedLang
        {
            get => _selectedLang;
            set
            {
                _selectedLang = value;
                ChangeLanguage(_selectedLang);
                OnPropertyChanged();
            }
        }

        private bool _allNotifications;
        public bool AllNotifications
        {
            get => _allNotifications;
            set
            {
                if (_allNotifications.Equals(value))
                    return;

                _allNotifications = value;
                Properties.UserSettings.Default.AllNotifications = value;
                OnPropertyChanged();
            }
        }

        private FilterInfo _currentVideoDevice;
        public FilterInfo CurrentVideoDevice
        {
            get => _currentVideoDevice;
            set
            {
                if (ReferenceEquals(_currentVideoDevice, value))
                    return;

                _currentVideoDevice = value;
                OnPropertyChanged();
            }
        }

        private AudioDeviceInfo _currentAudioDevice;
        public AudioDeviceInfo CurrentAudioDevice
        {
            get => _currentAudioDevice;
            set
            {
                if (ReferenceEquals(_currentAudioDevice, value))
                    return;

                _currentAudioDevice = value;
                OnPropertyChanged();
            }
        }

        private void ChangeLanguage(string lang)
        {
            Properties.UserSettings.Default.Lang = lang;
            Client.SetLanguage(lang);
        }

        private void GetVideoDevices()
        {
            var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            foreach (var device in devices)
            {
                VideoDevices.Add(device);
            }

            CurrentVideoDevice = VideoDevices.FirstOrDefault(x => string.Equals(x.MonikerString, Properties.UserSettings.Default.WebCamera));

            if (CurrentVideoDevice is null && VideoDevices.Any())
                CurrentVideoDevice = VideoDevices[0];
        }

        private void GetAudioDevices()
        {
            //var devices = new FilterInfoCollection(FilterCategory.AudioInputDevice);

            var devices = new AudioDeviceCollection(AudioDeviceCategory.Capture);

            foreach (var device in devices)
            {
                //if (device.Guid.ToString().StartsWith("0"))
                //    continue;

                AudioDevices.Add(device);
            }

            CurrentAudioDevice = AudioDevices.FirstOrDefault(x => string.Equals(x.Guid.ToString(), Properties.UserSettings.Default.Microphone));

            if (CurrentAudioDevice is null && AudioDevices.Any())
                CurrentAudioDevice = AudioDevices[0];
        }

        public override void OnLostSelection()
        {
            Properties.UserSettings.Default.Microphone = CurrentAudioDevice?.Guid.ToString();
            Properties.UserSettings.Default.WebCamera = CurrentVideoDevice?.MonikerString;
            Properties.UserSettings.Default.Save();
            AudioDevices.Clear();
            VideoDevices.Clear();
        }

        public override void OnSelected()
        {
            GetVideoDevices();
            GetAudioDevices();
        }
    }
}
