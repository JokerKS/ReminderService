using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Code11.Model;
using Code11.Model.Extensions;
using System.Threading;

namespace Code11
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerCall)]
    public class ReminderService : IReminder
    {
        #region Callbacks
        internal static Dictionary<int, IReminderCallback> Callbacks = new Dictionary<int, IReminderCallback>(); 
        #endregion


        #region Login()
        public User Login(string username, string password)
        {
            try
            {
                using (var db = new AppContext())
                {
                    var user = db.Users.Where(x => x.UserName == username && x.Password == password).SingleOrDefault();
                    if (user != null)
                    {
                        //Get callback funktion and update it when exists
                        IReminderCallback callback = OperationContext.Current.GetCallbackChannel<IReminderCallback>();
                        if (Callbacks.ContainsKey(user.Id))
                        {
                            Callbacks[user.Id] = callback;
                        }
                        else
                        {
                            Callbacks.Add(user.Id, callback);
                        }

                        //Update login date
                        user.LastLoginDate = DateTime.Now;
                        db.Entry(user).Property(x => x.LastLoginDate).IsModified = true;
                        db.SaveChanges();

                        var sendNotReseivedThread = new Thread(new ThreadStart(() => ReminderManager.SendNotReseived(user.Id)));
                        sendNotReseivedThread.IsBackground = true;
                        sendNotReseivedThread.Start();

                        return user;
                    }
                    else
                    {
                        throw new FaultException<NotCorrectUsernameOrPassword>(
                            new NotCorrectUsernameOrPassword(string.Format("User with username '{0}' and password '{1}' was not found.", username, password)), 
                            new FaultReason("User with such username and password was not found. Please check the spelling of the username and password."));
                    }                        
                }
            }
            catch (FaultException<NotCorrectUsernameOrPassword> e)
            {
                throw e;
            }
            catch (Exception)
            {
                return new User();
            }
        }
        #endregion

        #region SignOut()
        public bool SignOut(int userId)
        {
            try
            {
                Callbacks.Remove(userId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        } 
        #endregion

        #region Register()
        public bool Register(string username, string password, string email)
        {
            try
            {
                User newUser = new User(username, password, string.IsNullOrWhiteSpace(email) ? null : email);
                if (newUser.IsValid())
                {
                    using (var db = new AppContext())
                    {
                        newUser.LastLoginDate = new DateTime(1964, 11, 03);
                        newUser.RegisterDate = DateTime.Now;
                        db.Users.Add(newUser);
                        db.SaveChanges();
                    }
                    return true;
                }
                else
                {
                    throw new FaultException<NotCorrectUsernameOrPassword>(
                        new NotCorrectUsernameOrPassword(string.Format("User with username '{0}' or password '{1}' is not correct.", username, password)),
                        new FaultReason("Username or password is not correct. Please check the spelling of the username and password."));
                }
            }
            catch (FaultException<NotCorrectUsernameOrPassword> e)
            {
                throw e;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion

        #region UpdateUser()
        public bool UpdateUser(User user)
        {
            try
            {
                if (user == null)
                    throw new FaultException<ArgumentNullException>(new ArgumentNullException("User can't be null"));

                if (user.IsValid())
                {
                    using (var db = new AppContext())
                    {
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    return true;
                }
                return false;
            }
            catch (FaultException<ArgumentNullException> e)
            {
                throw e;
            }
            catch (Exception)
            {

                return false;
            }
        }
        #endregion

        #region RemoveUser()
        public bool RemoveUser(int userId)
        {
            try
            {
                using (var db = new AppContext())
                {
                    var user = db.Users.Find(userId);
                    if (user != null)
                    {
                        db.Entry(user).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        throw new FaultException<UserNotFound>(
                           new UserNotFound(string.Format("User with id '{0}'was not found.", userId)),
                           new FaultReason("User with such id was not found. Please check the spelling of the id."));
                    }
                }
            }
            catch (FaultException<UserNotFound> e)
            {
                throw e;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion


        #region GetReminder()
        public Reminder GetReminder(int userId, int reminderId)
        {
            try
            {
                using (var db = new AppContext())
                {
                    var reminder = db.Reminders.Where(x => x.Id == reminderId && x.UserId == userId).SingleOrDefault();
                    if(reminder != null)
                    {
                        reminder.ReminderType = (ReminderTypeEnum)reminder.TypeNumber;
                    }
                    return reminder;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region GetAllReminders()
        public List<Reminder> GetAllReminders(int userId)
        {
            try
            {
                using (var db = new AppContext())
                {
                    var reminders = db.Reminders.Where(x => x.UserId == userId).ToList();
                    reminders.ForEach(x => x.ReminderType = (ReminderTypeEnum)x.TypeNumber);

                    return reminders;
                }
            }
            catch (Exception e)
            {
                return new List<Reminder>();
            }
        }
        #endregion


        #region AddReminder()
        public int AddReminder(int userId, Reminder reminder)
        {
            try
            {
                if (reminder == null)
                    throw new ArgumentNullException("Reminder can't be null");

                using (var db = new AppContext())
                {
                    if (reminder.IsValid())
                    {
                        reminder.UserId = userId;
                        reminder.TypeNumber = (int)reminder.ReminderType;

                        if(reminder.ReminderType == ReminderTypeEnum.Once && reminder.DateAndTime < DateTime.Now)
                        {
                            throw new FaultException<NotCorrectDataAndTime>(
                            new NotCorrectDataAndTime("Date and time of reminder was passed."),
                            new FaultReason("Date and time of reminder was passed. Please specify the correct date and time."));
                        }

                        db.Reminders.Add(reminder);
                        db.SaveChanges();
                    }
                }
                return reminder.Id;
            }
            catch (FaultException<NotCorrectDataAndTime> e)
            {
                throw e;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        #endregion

        #region UpdateReminder()
        public bool UpdateReminder(int userId, int reminderId, Reminder reminder)
        {
            try
            {
                if (reminder == null)
                {
                    throw new FaultException<ReminderNotFound>(
                            new ReminderNotFound("Reminder in parameters can not be null."),
                            new FaultReason("Reminder in parameters can not be null."));
                }

                using (var db = new AppContext())
                {
                    var reminderToUpdate = db.Reminders.Where(x => x.Id == reminderId && x.UserId == userId).SingleOrDefault();
                    if (reminderToUpdate != null)
                    {
                        reminderToUpdate.Name = reminder.Name;
                        reminderToUpdate.Description = reminder.Description;
                        reminder.TypeNumber = (int)reminder.ReminderType;
                        reminder.DateAndTime = reminder.DateAndTime;
                        reminder.Days = reminder.Days;

                        db.Entry(reminderToUpdate).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        throw new FaultException<ReminderNotFound>(
                            new ReminderNotFound("Reminder with this id not found."),
                            new FaultReason("Reminder with this id not found."));
                    }
                }
            }
            catch (FaultException<ReminderNotFound> e)
            {
                throw e;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region RemoveReminder()
        public bool RemoveReminder(int userId, int reminderId)
        {
            try
            {
                using (var db = new AppContext())
                {
                    var reminderToDelete = db.Reminders.Where(x => x.Id == reminderId && x.UserId == userId).SingleOrDefault();
                    db.Entry(reminderToDelete).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }
        }
        #endregion


        #region ActivateReminder()
        public bool ActivateReminder(int userId, int reminderId)
        {
            try
            {
                return ChangeReminderState(userId, reminderId, true);
            }
            catch (FaultException<ReminderNotFound> e)
            {
                throw e;
            }
            catch (Exception)
            {

                return false;
            }
        }
        #endregion

        #region DeactivateReminder()
        public bool DeactivateReminder(int userId, int reminderId)
        {
            try
            {
                return ChangeReminderState(userId, reminderId, false);
            }
            catch (FaultException<ReminderNotFound> e)
            {
                throw e;
            }
            catch (Exception)
            {

                return false;
            }
        }
        #endregion

        #region ConfirmReceipt()
        public void ConfirmReceipt(int reminderId)
        {
            using (var db = new AppContext())
            {
                var reminder = db.Reminders.Where(x => x.Id == reminderId).SingleOrDefault();
                if (reminder != null)
                {
                    reminder.Status = ReminderStatus.Received();
                    db.Entry(reminder).Property(x => x.Status).IsModified = true;
                    db.SaveChanges();
                }
            }
        } 
        #endregion


        #region private ChangeReminderState()
        private bool ChangeReminderState(int userId, int reminderId, bool isActive)
        {
            try
            {
                using (var db = new AppContext())
                {
                    var reminder = db.Reminders.Where(x => x.Id == reminderId && x.UserId == userId).SingleOrDefault();
                    if (reminder != null)
                    {
                        reminder.IsActive = isActive;
                        db.Entry(reminder).Property(x => x.IsActive).IsModified = true;
                        db.SaveChanges();

                        if (isActive)
                        {
                            reminder.ReminderType = (ReminderTypeEnum)reminder.TypeNumber;
                            if (reminder.DateAndTime <= DateTime.Now.AddHours(13))
                            {
                                ReminderManager.AddToQueue(reminder);
                            }
                        }
                        else
                        {
                            ReminderManager.RemoveFromQueue(reminder.Id);
                        }

                        return true;
                    }
                    else
                    {
                        throw new FaultException<ReminderNotFound>(
                           new ReminderNotFound(string.Format("Reminder with id '{0}' and userId '{1}' was not found.", reminderId, userId)),
                           new FaultReason("Reminder with such id and userId was not found. Please check the spelling of the id."));
                    }
                }
            }
            catch (FaultException<ReminderNotFound> e)
            {
                throw e;
            }
            catch (Exception)
            {

                return false;
            }
        }
        #endregion

    }
}
