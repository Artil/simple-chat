using ChatCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatDesktop.ViewModels.Dialogs
{
    public class BaseDialogVM : DataErrorInfoVM
    {
        public BaseDialogVM(string text, bool isBool = false)
        {
            Text = text;
            IsBool = isBool;
        }

        private string _header;
        public string Header
        {
            get => _header;
            set
            {
                if (_header.Equals(value))
                    return;

                _header = value;
                OnPropertyChanged();
            }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (String.Equals(_text, value))
                    return;

                _text = value;
                OnPropertyChanged();
            }
        }

        private bool _isBool;
        public bool IsBool
        {
            get => _isBool;
            set
            {
                if (_isBool.Equals(value))
                    return;

                _isBool = value;
                OnPropertyChanged();
            }
        }
    }
}
