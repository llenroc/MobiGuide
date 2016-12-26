using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for EditProfileWindow.xaml
    /// </summary>
    public partial class EditProfileWindow : Window
    {
        private static string newName = String.Empty;
        static string connectionString = String.Empty;
        public static string NewName
        {
            get
            {
                return newName;
            }
        }
        public EditProfileWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
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
            if (e.Key == Key.Enter)
            {
                saveBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            base.OnPreviewKeyDown(e);
        }
        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResourceDictionary res = Application.Current.Resources;
            uNameTxtBlock.Text = res["UserLogon"].ToString();
            alNameTxtBlock.Text = res["AirlineName"].ToString();
            firstNameTxtBox.Text = res["FirstName"].ToString();
            lastNameTxtBox.Text = res["LastName"].ToString();
            firstNameTxtBox.Focus();
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!newPwdBox.Password.Equals(cfmPwdBox.Password))
            {
                MessageBox.Show("Please match your new password", "ERROR");
            } else
            {
                string cfmMsg = String.Format("Do you want to save changes?" + System.Environment.NewLine + System.Environment.NewLine +
                    "First Name : {0}" + System.Environment.NewLine +
                    "Last Name : {1}" + System.Environment.NewLine +
                    "Password : {2}", firstNameTxtBox.Text, lastNameTxtBox.Text,
                    newPwdBox.Password.Length == 0 ? "(unchanged)" : new string('*', newPwdBox.Password.Length));
                MessageBoxResult result = MessageBox.Show(cfmMsg, "Confirm?", MessageBoxButton.OKCancel);
                if(result == MessageBoxResult.OK)
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("firstName", firstNameTxtBox.Text);
                    data.Add("lastName", lastNameTxtBox.Text);
                    data.Add("password", cfmPwdBox.Password);
                    if (await updateProfile(data))
                    {
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window.GetType() == typeof(MainWindow))
                            {
                                MobiGuide.UserInfo userInfo = new MobiGuide.UserInfo();
                                userInfo.FullName = String.Format("{0} {1}", data["firstName"], data["lastName"]);
                                (window as MainWindow).DataContext = userInfo;
                            }
                        }
                        Application.Current.Resources["FirstName"] = data["firstName"];
                        Application.Current.Resources["LastName"] = data["lastName"];
                        this.Hide();
                        MessageBox.Show("Update Profile Succussfully.", "SUCCESS");
                        this.Close();
                    } else
                    {
                        MessageBox.Show("Failed to update profile.", "ERROR");
                        this.Close();
                    }
                }
            }
        }

        private static async Task<bool> updateProfile(Dictionary<string, string> data)
        {
            bool result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand(String.Format("UPDATE UserAccount " +
                            "SET FirstName = '{0}', LastName = '{1}'{2} " +
                            "WHERE UserAccountId = '{3}'", data["firstName"], data["lastName"],
                            String.IsNullOrWhiteSpace(data["password"]) ? "" : ", UserPassword = '" + data["password"] + "'",
                            Application.Current.Resources["UserAccountId"].ToString()), con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Result");
                    return false;
                }
            });
            return result;
        }
    }
}
