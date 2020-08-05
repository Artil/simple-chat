using System;
using System.Collections.Generic;
using System.Text;
using ToastNotifications;
using ToastNotifications.Core;

namespace ChatDesktop.Notifications
{
    public static class NotificationExtensions
    {
        public static NotifyMessage ShowNotification(this Notifier notifier, string title, string user, string color, string photoPath, string message, MessageOptions messageOptions = null)
        {
            var notificationViewModel = new NotifyMessage(title, user, color, photoPath, message, messageOptions);
            notifier.Notify(() => notificationViewModel);

            return notificationViewModel;
        }
    }
}
