using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatCore.Models
{
    public class BaseUserModel : ViewModelBase
    {
        public BaseUserModel()
        {
            Color = Colors.ElementAt(new Random().Next(Colors.Count));
        }

        private static List<string> Colors = new List<string>() { "#FF6341", "#FF4441", "#FFB441", "#41FFA2", "#41E9FF", "#417CFF", "#7E41FF", "#D441FF", "#FF4180", "#FFA17B", "#2EA91D", "#28978C" };

        private string _color;
        public string Color 
        {
            get => _color;
            set
            {
                if (String.Equals(_color, value))
                    return;

                _color = value;
                OnPropertyChanged();
            }
        }

        private string _photoPath;
        public string PhotoPath
        {
            get => _photoPath;
            set
            {
                if (String.Equals(_photoPath, value))
                    return;

                _photoPath = value;
                OnPropertyChanged();
            }
        }

        public string Name { get; set; }
        public string ShortName => Name?.Substring(0, 1).ToUpper();
    }
}
