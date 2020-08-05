using System;
using System.Collections.Generic;
using System.Text;

namespace ChatDesktop.ViewModels.Dialogs
{
    public class RemoveMessageDialogVM : BaseDialogVM
    {
        public RemoveMessageDialogVM(string text) : base(text)
        {
            IsBool = true;
        }

        private bool _removeType;
        public bool RemoveType
        {
            get => _removeType;
            set
            {
                if (_removeType.Equals(value))
                    return;

                _removeType = value;
            }
        }
    }
}
