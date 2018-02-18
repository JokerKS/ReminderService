using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
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
using VKozenko.ReminderClient;
using VKozenko.ReminderClient.Service;
using VKozenko.ReminderClient.Tools;

namespace ReminderClient
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            // TODO: validation
            var name = txtName.Text;
            var password = txtPassword.Password;
            var email = txtEmail.Text;

            if(string.IsNullOrEmpty(name) || name.Length < 5)
            {
                errName.Visibility = Visibility.Visible;
                return;
            }

            if (password == txtConfirmPassword.Password)
            {
                if (ServiceProxy.Register(name, password, email))
                {
                    var loginWindow = new Login(name);
                    loginWindow.Show();
                    this.Close();
                }
            }
            else
            {
                errConfirmPassword.Visibility = Visibility.Visible;
            }
        }

        private void CommandBinding_CanExecute_Close(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.GetPosition(this).Y <= 30)
            {
                DragMove();
            }
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool result = false;
            var email = (sender as TextBox).Text;
            if (string.IsNullOrEmpty(email))
            {
                result = true;
            }
            else if(ValidatorExtensions.IsValidEmailAddress(email))
            {
                result = true;
            }

            if(result)
            {
                errEmail.Visibility = Visibility.Collapsed;
            }
            else
            {
                errEmail.Visibility = Visibility.Visible;
            }
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(txtPassword.Password == txtConfirmPassword.Password)
            {
                errConfirmPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                errConfirmPassword.Visibility = Visibility.Visible;
            }
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var name = txtName.Text;
            if (string.IsNullOrEmpty(name) || name.Length < 5)
            {
                errName.Visibility = Visibility.Visible;
            }
            else
            {
                errName.Visibility = Visibility.Collapsed;
            }
        }
    }
}
