using System;
using System.Collections.Generic;
using System.Text;

namespace ChatDesktop.Interfaces
{
    public interface IClone<T>
    {
        T Clone<T>(T original) where T : new();
        T DeepClone<T>(T original) where T : new();
    }
}
