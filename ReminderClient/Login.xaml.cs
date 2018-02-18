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
    public partial class Login : Window
    {
        public Login():this(null)
        {
        }
        public Login(string name = null)
        {
            InitializeComponent();

            IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();
            StreamReader reader = new StreamReader(
                new IsolatedStorageFileStream("userInfo", FileMode.OpenOrCreate, isolatedStorage));

            if (reader.NotNull())
            {
                List<string> infos = new List<string>();
                while (!reader.EndOfStream)
                {
                    infos.Add(reader.ReadLine());
                }

                if(infos.Count >= 3 && infos[0] == "True")
                {
                    chkRememberMe.IsChecked = true;
                    txtName.Text = infos[1];
                    txtPassword.Password = infos[2];
                }
            }
            reader.Close();

            if(name.NotNull())
            {
                txtName.Text = name;
            }
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            // TODO: validation
            var name = txtName.Text;
            var password = txtPassword.Password;

            var user = ServiceProxy.Login(name, password);
            if(user.NotNull())
            {
                if (chkRememberMe.IsChecked.HasValue && chkRememberMe.IsChecked.Value)
                {
                    IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();

                    StreamWriter srWriter = new StreamWriter(new IsolatedStorageFileStream("userInfo", FileMode.Create, isolatedStorage));
                    srWriter.WriteLine(chkRememberMe.IsChecked.ToString());
                    srWriter.WriteLine(txtName.Text);
                    srWriter.WriteLine(txtPassword.Password);
                    srWriter.Flush();
                    srWriter.Close();
                }


                var mainWindow = new Main(user);
                mainWindow.Show();
                this.Close();
            }
        }

        private void chkRememberMe_Unchecked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();
            isolatedStorage.Remove();
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

        private void textRegister_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();
            isolatedStorage.Remove();

            var registerWindow = new Register();
            registerWindow.Show();
            this.Close();
        }
    }
}
