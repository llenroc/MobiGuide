using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using DatabaseConnector;
using MobiGuide.Class;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly DBConnector dbCon = new DBConnector();

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
                e.Handled = true;
            if(e.Key == Key.Enter)
                loginBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
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
                Close();
            }
        }

        private async Task<bool> doLogin(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(Messages.ERROR_EMPTY_UPASS, Captions.ERROR);
                return false;
            }
            DataRow result = await dbCon.GetDataRow("UserAccount", 
                new DataRow("UserLogon", username, "UserPassword", password));
            if (result.HasData && result.Error == ERROR.NoError)
            {
                Application.Current.Resources["UserAccountId"] = result.Get("UserAccountId");
                Application.Current.Resources["AirlineCode"] = result.Get("AirlineCode");
                return true;
            }
            if (!result.HasData && result.Error == ERROR.NoError)
            {
                MessageBox.Show(Messages.ERROR_UPASS_NOMATCH, Captions.WARNING);
                return false;
            }
            if (result.Error == ERROR.HasError)
            {
                MessageBox.Show(Messages.ERROR_LOGIN_FAILED, Captions.ERROR);
                return false;
            }
            MessageBox.Show(Messages.ERROR_LOGIN_FAILED, Captions.ERROR);
            return false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            usernameBox.Focus();
        }
    } 
}


