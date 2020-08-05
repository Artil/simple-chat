using ChatCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatDesktop.ViewModels.Base
{
    public class BaseContentVM : DataErrorInfoVM
    {
        public virtual void OnSelected() { }
        public virtual void OnLostSelection() { }
    }
}
