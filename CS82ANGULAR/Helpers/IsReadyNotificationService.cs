using System;

namespace CS82ANGULAR.Helpers
{
    public delegate void IsReadyNotification(Object sender, bool ready);
    public class IsReadyNotificationService
    {
        public event IsReadyNotification IsReadyEvent;
        public void DoNotify(Object sender, bool ready)
        {
            if (IsReadyEvent != null)
                IsReadyEvent(sender, ready);
        }
    }
}
