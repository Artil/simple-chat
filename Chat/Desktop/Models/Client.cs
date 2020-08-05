using ChatCore.Models;
using ChatDesktop.Enums;
using ChatDesktop.Resources.Lang;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace ChatDesktop.Models
{
    public class Client
    {
        public static string AccessToken;
        public static UserModel CurrentUser;
        public static string ServerKey;
        public static string ServerIV;

        public static Dictionary<string, string> UsersMedia = new Dictionary<string, string>();
        public static List<string> Notifications = new List<string>();

        public static event Action<BaseContentEnum, object> BaseContentChanged;

        private static BaseContentEnum _baseContent;

        public static void ChangeBaseContent(BaseContentEnum baseContent, object value)
        {
            _baseContent = baseContent;
            BaseContentChanged.Invoke(_baseContent, value);
        }

        public static BaseContentEnum BaseContent
        {
            get => _baseContent;
            set
            {
                _baseContent = value;

            }
        }

        public static void GetSavedMedia<T>(IEnumerable<T> list) where T : BaseUserModel
        {
            foreach (var media in list)
            {
                if (!UsersMedia.Any(x => x.Key == media.Name))
                {
                    if(String.IsNullOrEmpty(media.PhotoPath))
                        UsersMedia.Add(media.Name, media.Color);
                    else
                        UsersMedia.Add(media.Name, media.PhotoPath);
                }
                else
                {
                    var result = UsersMedia.FirstOrDefault(x => x.Key == media.Name).Value;
                    if (result.StartsWith("#"))
                        media.Color = result;
                    else
                        media.PhotoPath = result;
                }
            }
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

        public static void SetLanguage(string locale)
        {
            if (string.IsNullOrEmpty(locale)) locale = "";
            var culture = new CultureInfo(locale);
            Localization.Instance.CurrentCulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            Properties.UserSettings.Default.Lang = locale;
            Properties.UserSettings.Default.Save();
        }
    }
}
