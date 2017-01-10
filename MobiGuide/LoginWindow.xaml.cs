using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Windows.Controls.Primitives;
using DatabaseConnector;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        DBConnector dbCon = new DBConnector();
        public LoginWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            if(e.Key == Key.Enter)
            {
                loginBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            base.OnPreviewKeyDown(e);
        }

        private async void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            loginBtn.IsEnabled = false;
            string username = usernameBox.Text;
            string password = passwordBox.Password;
            bool result = await doLogin(username, password);
            loginBtn.IsEnabled = true;
            if (result)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private async Task<bool> doLogin(string username, string password)
        {
            if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill both Username and Password", "Error");
                return false;
            }
            else
            {
                DataRow result = await dbCon.getDataRow("UserAccount", 
                    new DataRow("UserLogon", username, "UserPassword", password));
                if (result.HasData && result.Error == ERROR.NoError)
                {
                    Application.Current.Resources["UserAccountId"] = result.Get("UserAccountId");
                    Application.Current.Resources["AirlineCode"] = result.Get("AirlineCode");
                    return true;
                } else if (!result.HasData && result.Error == ERROR.NoError)
                {
                    MessageBox.Show("Username or Password do not match", "WARNING");
                    return false;
                } else if (result.Error == ERROR.HasError)
                {
                    MessageBox.Show("Failed to login, please contact administrator", "ERROR");
                    return false;
                } else
                {
                    MessageBox.Show("Failed to login, please contact administrator", "ERROR");
                    return false;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            usernameBox.Focus();
        }
    } 
}


