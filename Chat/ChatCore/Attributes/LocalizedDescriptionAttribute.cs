using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Resources;
using System.Text;

namespace ChatCore.Attributes
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        ResourceManager _resourceManager;
        string _resourceKey;
        public LocalizedDescriptionAttribute(string resourceKey, Type resourceType)
        {
            _resourceManager = new ResourceManager(resourceType);
            _resourceKey = resourceKey;
        }

        public override string Description
        {
            get
            {
                string description = _resourceManager.GetString(_resourceKey);
                return string.IsNullOrWhiteSpace(description) ? $"[[{_resourceKey}]]" : description;
            }
        }
    }
}
