using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VKozenko.ReminderClient.ServiceProxy;
using VKozenko.ReminderClient.Tools;

namespace VKozenko.ReminderClient
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private WindowContent status;

        private WindowContent Status
        {
            get { return status; }
            set
            {
                status = value;
                LoadData();
            }
        }
        private int SelectedReminderId { get; set; }
        private static List<Reminder> Reminders { get; set; }
        private static User User { get; set; }
        public Main(User user)
        {
            User = user;
            InitializeComponent();

            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Reminders = new List<Reminder>();

            foreach (var type in Enum.GetValues(typeof(ReminderType)))
            {
                cmbType.Items.Add(type);
            }

            var culture = new CultureInfo("pl-PL");
            foreach (var month in culture.DateTimeFormat.MonthNames.Take(12))
            {
                cmbMonths.Items.Add(month.FirstLetterToUpper());
            }
            cmbMonths.SelectedIndex = DateTime.Now.Month-1;
            cmbMonths.SelectionChanged += cmbMonths_SelectionChanged;

            grdCalendarView.RowDefinitions.Add(new RowDefinition());
            
            for (int i = 0; i < 7; i++)
            {
                grdCalendarView.ColumnDefinitions.Add(new ColumnDefinition() { MaxWidth = 50 });
            }

            Status = WindowContent.Reminders;
        }

        private void LoadData()
        {
            grdContent.Visibility = spAddReminder.Visibility = spCalendar.Visibility = Visibility.Collapsed;
            
            switch (Status)
            {
                case WindowContent.Reminders:
                    LoadReminders();
                    break;
                case WindowContent.AddReminder:
                    LoadAddForm();
                    break;
                case WindowContent.ReminderDetails:
                    LoadDetails();
                    break;
                case WindowContent.Calendar:
                    LoadCalendar();
                    break;
                case WindowContent.UserDetails:
                    break;
                default:
                    break;
            }
        }

        private void LoadReminders()
        {
            lblMainTitle.Text = "Przypomnienia";

            Reminders = Service.ServiceProxy.GetReminders(User);
            var rowCount = (Reminders.Count + 1) / 2;

            RowDefinition row;
            grdContent.Children.Clear();
            grdContent.RowDefinitions.Clear();
            for (int i = 0; i < rowCount; i++)
            {
                row = new RowDefinition() {
                    Name = $"row_{i+1}"
                };
                grdContent.RowDefinitions.Add(row);
            }

            StackPanel sp;
            TextBlock lblTitle, lblDate;
            var itemNumber = 0;
            foreach (var reminder in Reminders)
            {
                sp = new StackPanel() {
                    Name = $"rem_{reminder.Id}",
                    Margin = new Thickness(5, 5, 0, 0),
                    Cursor = Cursors.Hand
                };

                sp.AddHandler(StackPanel.MouseDownEvent, new MouseButtonEventHandler(Reminder_Click));

                lblTitle = new TextBlock() {
                    Text = reminder.Name,
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap,
                };
                sp.Children.Add(lblTitle);

                lblDate = new TextBlock()
                {
                    Text = reminder.DateAndTime.ToString("dd MMMM yyyy, HH mm")
                };
                sp.Children.Add(lblDate);

                Grid.SetRow(sp, itemNumber / 2);
                if (itemNumber % 2 != 0)
                {
                    Grid.SetColumn(sp, 1);
                }
                itemNumber++;

                grdContent.Children.Add(sp);
            }
            grdContent.Visibility = Visibility.Visible;
        }

        private void Reminder_Click(object sender, MouseButtonEventArgs e)
        {
            SelectedReminderId = Convert.ToInt32((sender as StackPanel).Name.Remove(0, 4));
            Status = WindowContent.ReminderDetails;
        }

        private void LoadAddForm()
        {
            lblMainTitle.Text = "Dodanie przypomnienia";
            spAddReminder.Visibility = Visibility.Visible;
        }

        private void LoadDetails()
        {
            lblMainTitle.Text = "Szczegóły przypomnienie";

            var rem = Reminders.FirstOrDefault(x => x.Id == SelectedReminderId);
            if (rem.NotNull())
            {
                txtName.Text = rem.Name;
                txtDescriptions.Text = rem.Description;
                cmbType.SelectedIndex = (int)rem.ReminderType - 1;
                calendarDate.SelectedDate = rem.DateAndTime.Date;
                txtTime.Text = $"{rem.DateAndTime.Hour}:{rem.DateAndTime.Minute}";
            }

            spAddReminder.Visibility = Visibility.Visible;
        }

        private void LoadCalendar(DateTime? date = null)
        {
            DateTime dateNow;
            TextBlock txtDay;

            if (!date.HasValue)
            {
                lblMainTitle.Text = "Kalendarz przypomnień";
                dateNow = DateTime.Now.Date;
            }
            else
            {
                dateNow = date.Value;
            }

            grdCalendarView.Children.Clear();
            if (grdCalendarView.RowDefinitions.Count > 1)
            {
                grdCalendarView.RowDefinitions.RemoveRange(1, grdCalendarView.RowDefinitions.Count - 1);
            }

            var culture = new CultureInfo("pl-PL");
            var daysAbbrev = culture.DateTimeFormat.AbbreviatedDayNames.ToList();
            string temp = daysAbbrev[0];
            daysAbbrev.RemoveAt(0);
            daysAbbrev.Add(temp);
            for (int i = 0; i < 7; i++)
            {
                txtDay = new TextBlock()
                {
                    Text = daysAbbrev[i].FirstLetterToUpper(),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetColumn(txtDay, i);
                grdCalendarView.Children.Add(txtDay);
            }

            var daysInMonth = DateTime.DaysInMonth(dateNow.Year, dateNow.Month);

            txtYear.Text = dateNow.Year.ToString();

            var dayOfWeek = (int)(new DateTime(dateNow.Year, dateNow.Month, 1).DayOfWeek + 6) % 7;

            int currentDay = 0, row = 1;
            bool firstDay = true;
            while(currentDay < daysInMonth)
            {
                grdCalendarView.RowDefinitions.Add(new RowDefinition());
                for (int i = 0; i < 7 && currentDay < daysInMonth; i++)
                {
                    txtDay = new TextBlock()
                    {
                        Text = (currentDay + 1).ToString(),
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };

                    if (firstDay && dayOfWeek == i)
                    {
                        currentDay++;

                        Grid.SetRow(txtDay, row);
                        Grid.SetColumn(txtDay, i);
                        grdCalendarView.Children.Add(txtDay);

                        firstDay = false;
                        continue;
                    }
                    
                    if(!firstDay)
                    {
                        currentDay++;

                        Grid.SetRow(txtDay, row);
                        Grid.SetColumn(txtDay, i);
                        grdCalendarView.Children.Add(txtDay);
                    }
                }
                row++;
            }
            spCalendar.Visibility = Visibility.Visible;
        }

        private void btnAddReminder_Click(object sender, RoutedEventArgs e)
        {
            Reminder rem = new Reminder();
            rem.Name = txtName.Text;
            rem.Description = txtDescriptions.Text;
            rem.ReminderType = (ReminderType)((int)cmbType.SelectedValue);
            if (calendarDate.SelectedDate.HasValue)
            {
                rem.DateAndTime = calendarDate.SelectedDate.Value;
                var time = txtTime.Text.Split(':');
                rem.DateAndTime = rem.DateAndTime.AddHours(Convert.ToDouble(time[0])).AddMinutes(Convert.ToDouble(time[1]));
            }

            if (Status == WindowContent.AddReminder)
            {
                int remId = Service.ServiceProxy.AddReminder(User.Id, rem);
                if (remId > 0)
                {
                    Service.ServiceProxy.ActivateReminder(User.Id, remId);
                    Status = WindowContent.Reminders;
                }
                else
                {
                    throw new NotImplementedException("AddReminder not success");
                }
            }
            else if (Status == WindowContent.ReminderDetails)
            {
                rem.Id = SelectedReminderId;
                if (Service.ServiceProxy.UpdateReminder(User.Id, rem))
                {
                    SelectedReminderId = new int();

                    Service.ServiceProxy.ActivateReminder(User.Id, rem.Id);
                    Status = WindowContent.Reminders;
                }
                else
                {
                    throw new NotImplementedException("UpdateReminder not success");
                }
            }
        }

        #region Left menu buttons

        #region btnAdd_Click()
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Status = WindowContent.AddReminder;
        }
        #endregion

        #region btnShowAll_Click
        private void btnShowAll_Click(object sender, RoutedEventArgs e)
        {
            Status = WindowContent.Reminders;
        }
        #endregion

        #region btnCalendar_Click()
        private void btnCalendar_Click(object sender, RoutedEventArgs e)
        {
            Status = WindowContent.Calendar;
        }
        #endregion

        #region btnSettings_Click()
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        } 
        #endregion

        #endregion

        private void cmbMonths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var date = new DateTime(Convert.ToInt32(txtYear.Text), cmbMonths.SelectedIndex + 1, 1);
            LoadCalendar(date);
        }
    }

    internal enum WindowContent
    {
        Reminders = 1,
        AddReminder = 2,
        ReminderDetails = 3,
        Calendar = 4,
        UserDetails = 5
    }
}
