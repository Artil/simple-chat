using ChatDesktop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ChatDesktop.Resources.Controls
{
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl, INotifyPropertyChanged
    {
        public VideoPlayer()
        {
            InitializeComponent();
            Media.Stop();
        }


        private ICommand _stopStart;
        public ICommand StopStartCommand => _stopStart == null ? _stopStart = new RelayCommand(StopStart) : _stopStart;

        private double _max;
        public double Max
        {
            get => _max;
            set
            {
                if (_max.Equals(value))
                    return;

                _max = value;
                OnPropertyChanged();
            }
        }

        private double _position;
        public double Position
        {
            get => _position;
            set
            {
                if (_position.Equals(value))
                    return;

                Media.Position = TimeSpan.FromSeconds(value);
                _position = value;
                OnPropertyChanged();
            }
        }

        private bool _isStarted;
        public bool IsStarted
        {
            get => _isStarted;
            set
            {
                if (_isStarted.Equals(value))
                    return;

                _isStarted = value;
                OnPropertyChanged();
            }
        }

        public static readonly DependencyProperty MediaPathProperty = DependencyProperty.Register("MediaPath", typeof(String), typeof(VideoPlayer));
        public string MediaPath
        {
            get => (String)this.GetValue(MediaPathProperty);
            set { this.SetValue(MediaPathProperty, value); }
        }

        private void Init()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Position = Media.Position.TotalSeconds;
            Max = Media.NaturalDuration.HasTimeSpan ? Media.NaturalDuration.TimeSpan.TotalSeconds : 0;

            if (Media.NaturalDuration.HasTimeSpan && TimeSpan.Equals(Media.Position, Media.NaturalDuration.TimeSpan))
            {
                Media.Stop();
                IsStarted = !IsStarted;
            }

        }

        private void StopStart(object obj)
        {
                Init();

            if (IsStarted)
            {
                Media.Pause();
                IsStarted = !IsStarted;
            }
            else
            {
                Media.Play();
                IsStarted = !IsStarted;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
