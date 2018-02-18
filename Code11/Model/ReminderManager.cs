using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Code11.Model
{
    public static class ReminderManager
    {
        private static List<Reminder> RemindersQueue = new List<Reminder>();

        internal static void AddToQueue(Reminder reminder)
        {
            lock (RemindersQueue)
            {
                RemindersQueue.Add(reminder);
            }
        }

        internal static void RemoveFromQueue(int reminderId)
        {
            lock (RemindersQueue)
            {
                RemindersQueue.Remove(RemindersQueue.FirstOrDefault(x => x.Id == reminderId));
            }
        }

        internal static void CheckUpdate()
        {
            List<Reminder> remindersToSend;
            while (true)
            {
                remindersToSend = new List<Reminder>();
                foreach (var item in RemindersQueue)
                {
                    bool sendUpdate = false;
                    switch (item.ReminderType)
                    {
                        case ReminderTypeEnum.EveryDay:
                            if (DateTime.Now.TimeOfDay >= item.DateAndTime.TimeOfDay)
                            {
                                sendUpdate = true;
                            }
                            break;
                        case ReminderTypeEnum.EveryWeek:
                            string[] days = item.Days.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            DateTime.Now.DayOfWeek.ToString();
                            if (DateTime.Now.TimeOfDay >= item.DateAndTime.TimeOfDay
                                && days.Contains(DateTime.Now.DayOfWeek.ToString("ddd")))
                            {
                                sendUpdate = true;
                            }
                            break;
                        case ReminderTypeEnum.EveryYear:
                            if (DateTime.Now >= item.DateAndTime)
                            {
                                sendUpdate = true;
                            }
                            break;
                        case ReminderTypeEnum.Once:
                            if (DateTime.Now >= item.DateAndTime)
                            {
                                sendUpdate = true;
                            }
                            break;
                        default:
                            break;
                    }

                    if (sendUpdate)
                    {
                        remindersToSend.Add(item);
                    }
                }

                foreach (var item in remindersToSend)
                {
                    var sendThread = new Thread(new ThreadStart(() => SendUpdateToClient(item)));
                    sendThread.IsBackground = true;
                    sendThread.Start();
                }
                
                Thread.Sleep(500);
            }
        }

        private static void SendUpdateToClient(Reminder reminder)
        {
            string status = null;
            try
            {
                lock (RemindersQueue)
                {
                    RemindersQueue.Remove(reminder);
                }

                if (ReminderService.Callbacks.ContainsKey(reminder.UserId))
                {
                    ReminderService.Callbacks[reminder.UserId].ExecuteReminder(reminder);
                    status = ReminderStatus.Sent();
                }
            }
            catch (TimeoutException)
            {
                status = ReminderStatus.TimeoutError();
            }
            catch (Exception)
            {
                status = ReminderStatus.Error();
            }

            using (var db = new AppContext())
            {
                reminder.Status = status;
                db.Reminders.Attach(reminder);
                db.Entry(reminder).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
            }

            Thread.CurrentThread.Abort();
        }

        internal static void CheckAddingToQueue()
        {
            bool firstTime = true;
            while (true)
            {
                var dtNow = DateTime.Now;
                float times = 0;

                using (var db = new AppContext())
                {
                    var remIds = RemindersQueue.Select(x => x.Id);
                    var status = ReminderStatus.Received();
                    var reminders = db.Reminders
                        .Where(x => x.Status != status && 
                            x.IsActive && 
                            x.DateAndTime <= dtNow && 
                            !remIds.Contains(x.Id))
                        .ToList();

                    reminders = reminders.Where(x => ReminderService.Callbacks.ContainsKey(x.UserId)).ToList();

                    reminders.ForEach(x => x.ReminderType = (ReminderTypeEnum)x.TypeNumber);

                    var remindersForOnce = reminders.Where(x => x.TypeNumber == (int)ReminderTypeEnum.Once);
                    foreach (var item in remindersForOnce)
                    {
                        if (item.DateAndTime <= dtNow.AddMinutes(5))
                        {
                            AddToQueue(item);
                        }
                    }

                    times += 2.5F;
                    if (times % 2.5 == 0 || firstTime)
                    {
                        Thread.Sleep(150000);
                        continue;
                    }
                    if(times % 60 == 0 || firstTime)
                    {
                        var remindersForEveryDay = reminders.Where(x => x.TypeNumber == (int)ReminderTypeEnum.EveryDay);
                        foreach (var item in remindersForEveryDay)
                        {
                            if (item.DateAndTime <= dtNow.AddMinutes(65))
                            {
                                AddToQueue(item);
                            }
                        }
                    }
                    if (times % 720 == 0 || firstTime)
                    {
                        var newDT = dtNow.AddHours(13);
                        var remindersForEveryWeek = reminders.Where(x => x.TypeNumber == (int)ReminderTypeEnum.EveryWeek);
                        foreach (var item in remindersForEveryWeek)
                        {
                            if (item.DateAndTime <= newDT && item.Days.Contains(newDT.DayOfWeek.ToString("ddd")))
                            {
                                AddToQueue(item);
                            }
                        }
                        var remindersForEveryYear = reminders.Where(x => x.TypeNumber == (int)ReminderTypeEnum.EveryYear);
                        foreach (var item in remindersForEveryYear)
                        {
                            if (item.DateAndTime.Date <= newDT.Date)
                            {
                                AddToQueue(item);
                            }
                        }
                        times = 0;
                    }

                    if (firstTime)
                        firstTime = false;
                }
            }
        }

        public static void SendNotReseived(int userId)
        {
            using (var db = new AppContext())
            {
                var tmpDT = DateTime.Now.AddMinutes(5);
                var remIds = RemindersQueue.Select(x => x.Id);
                var status = ReminderStatus.Received();
                var reminders = db.Reminders
                        .Where(x => x.UserId == userId && 
                            x.Status != status && 
                            x.DateAndTime <= tmpDT &&
                            !remIds.Contains(x.Id))
                        .ToList();
                if (reminders.Count > 0)
                {
                    reminders.ForEach(x => x.ReminderType = (ReminderTypeEnum)x.TypeNumber);
                    lock (RemindersQueue)
                    {
                        RemindersQueue.AddRange(reminders);
                    }
                }

                if(Thread.CurrentThread.IsBackground)
                {
                    Thread.CurrentThread.Abort();
                }
            }
        }
    }
}