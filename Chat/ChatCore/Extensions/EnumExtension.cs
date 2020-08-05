using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ChatCore.Extensions
{
    public static class EnumExtension
    {
        public static string GetDisplayDescription(this Enum enumValue)
        {
            return enumValue.GetType().GetMember(enumValue.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>()?
                .GetDescription() ?? enumValue.ToString();
        }
    }
}
