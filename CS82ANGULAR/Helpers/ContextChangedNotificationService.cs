using System;

namespace CS82ANGULAR.Helpers
{
    public class ContextChangedNotificationService
    {
        public delegate void ContextChangedNotification(Object sender);
        public class ContextChangedService
        {
            public event ContextChangedNotification ContextChanged;
            public void DoNotify(Object sender)
            {
                if (ContextChanged != null)
                    ContextChanged(sender);
            }
        }
    }
}
