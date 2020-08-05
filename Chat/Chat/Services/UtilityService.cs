using ChatDbCore;
using ChatDbCore.Account;
using ChatDbCore.ChatModels;
using ChatServer.Hubs;
using ChatServer.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatServer.Services
{
    public class UtilityService
    {
        private readonly Context _context;
        public UtilityService(Context context)
        {
            _context = context;
        }

        public bool IsMessageDeleted(Message message)
        {
            return _context.MessagesStatus.FirstOrDefault(y => String.Equals(y.MessageId, message.Id)
            && String.Equals(y.User.UserName, message.User.UserName))?.IsDeleted ?? false;
        }

        // set values for same name and type
        public static void SetValues(object obj, object value)
        {
            foreach (var originalProp in obj.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                var prop = value.GetType().GetProperty(originalProp.Name);
                if (!ReferenceEquals(prop, null) && originalProp.PropertyType == prop.PropertyType)
                    originalProp.SetValue(obj, prop.GetValue(value));
            }
        }
    }
}
