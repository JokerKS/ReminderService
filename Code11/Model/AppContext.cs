using System;
using System.Data.Entity;

namespace Code11.Model
{
    public class AppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public AppContext() : base("ReminderDbConnection")
        {
        }
    }
}