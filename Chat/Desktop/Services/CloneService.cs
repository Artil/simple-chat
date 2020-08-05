using ChatDesktop.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatDesktop.Services
{
    public class CloneService<T> : IClone<T>
    {
        public T Clone<T>(T original) where T : new()
        {
            T clone = new T();

            foreach (var originalProp in original.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                originalProp.SetValue(clone, originalProp.GetValue(original));
            }

            return clone;
        }

        public T DeepClone<T>(T original) where T : new()
        {
            T clone = new T();

            foreach (var originalProp in original.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                if (originalProp.PropertyType.IsClass)
                    originalProp.SetValue(clone, DeepClone(originalProp.GetValue(original)));
                else
                    originalProp.SetValue(clone, originalProp.GetValue(original));
            }

            return clone;
        }
    }
}
