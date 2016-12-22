using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
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
                Debug.WriteLine("hell yeah");
            } else
            {
                Debug.WriteLine("dafuq");
            }
        }

        private static async Task<bool> doLogin(string username, string password)
        {
            bool result = await Task.Run(() =>
            {
                if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please fill both Username and Password", "Error");
                    return false;
                }
                else
                {
                    try
                    {
                        string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
                        using (SqlConnection con = new SqlConnection(connectionString))
                        {
                            SqlCommand cmd = new SqlCommand("SELECT * " +
                                "FROM UserAccount " +
                                "WHERE UserLogon = '" + username + "' " + 
                                "AND UserPassword = '" + password + "'", con);
                            con.Open();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    MessageBox.Show("Login Success", "Result");
                                    con.Close();
                                    return true;
                                }
                                else
                                {
                                    MessageBox.Show("Login Failed", "Result");
                                    con.Close();
                                    return false;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Login Error, Please contact administrator", "Result");
                        Debug.WriteLine(ex.ToString());
                        return false;
                    }
                }
            });
            return result;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            usernameBox.Focus();
        }
    } 
}


