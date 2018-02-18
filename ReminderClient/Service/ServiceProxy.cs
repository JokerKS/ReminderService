using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Proxy = VKozenko.ReminderClient.ServiceProxy;

namespace VKozenko.ReminderClient.Service
{
    internal static class ServiceProxy
    {
        static InstanceContext instance = new InstanceContext(new CallbackHandler());
        static Proxy.ReminderClient proxy = new Proxy.ReminderClient(instance);

        #region Login()
        internal static Proxy.User Login(string name, string password)
        {
            return proxy.Login(name, password);
        }
        #endregion

        #region Register()
        internal static bool Register(string name, string password, string email = null)
        {
            return proxy.Register(name, password, email);
        }
        #endregion

        #region SignOut()
        internal static bool SignOut(Proxy.User user)
        {
            return proxy.SignOut(user.Id);
        }
        #endregion

        #region GetReminders()
        internal static List<Proxy.Reminder> GetReminders(Proxy.User user)
        {
            var reminders = proxy.GetAllReminders(user.Id);
            return reminders.ToList();
        }
        #endregion

        #region AddReminder()
        internal static int AddReminder(int userId, Proxy.Reminder reminder)
        {
            return proxy.AddReminder(userId, reminder);
        }
        #endregion

        #region UpdateReminder()
        internal static bool UpdateReminder(int userId, Proxy.Reminder reminder)
        {
            return proxy.UpdateReminder(userId, reminder.Id, reminder);
        }
        #endregion

        #region ActivateReminder()
        internal static bool ActivateReminder(int userId, int reminderId)
        {
            return proxy.ActivateReminder(userId, reminderId);
        }
        #endregion
    }

    public sealed class CallbackHandler : Proxy.IReminderCallback
    {
        public void ExecuteReminder(Proxy.Reminder reminder)
        {
            App.ShowNotification(reminder.Name, reminder.Description);

            //throw new NotImplementedException();
        }
    }
}
