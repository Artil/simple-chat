using System;
using System.Collections.Generic;
using System.Text;

namespace ChatDesktop.Interfaces
{
    public interface IFileDragDropTarget
    {
        void OnFileDrop(string[] filepaths);
    }
}
