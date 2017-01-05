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
        ResourceDictionary res = Application.Current.Resources;
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
            uNameTxtBlock.Text = res["UserLogon"].ToString();
            alNameTxtBlock.Text = res["AirlineName"].ToString();
            firstNameTxtBox.Text = res["FirstName"].ToString();
            lastNameTxtBox.Text = res["LastName"].ToString();
            commitDateTxtBlock.Text = res["CommitDateTime"].ToString();
            loadCommitByUser();
            
            statusComboBox.Items.Add(new ComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new ComboBoxItem { Text = "Inactive", Value = "I" });

            for(int i = 0; i < statusComboBox.Items.Count; i++)
            {
                if(res["StatusCode"].ToString().Equals((statusComboBox.Items[i] as ComboBoxItem).Value))
                {
                    statusComboBox.SelectedIndex = i;
                    break;
                }
            }
            firstNameTxtBox.Focus();
        }

        private async void loadCommitByUser()
        {
            commitByTxtBlock.Text = await getCommitUser(res["CommitBy"].ToString());
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
                    data.Add("statusCode", (statusComboBox.SelectedItem as ComboBoxItem).Value.ToString());
                    data.Add("password", cfmPwdBox.Password);
                    if (await updateProfile(data))
                    {
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window.GetType() == typeof(MainWindow))
                            {
                                MobiGuide.UserInfo userInfo = (MobiGuide.UserInfo)window.DataContext;
                                userInfo.FullName = String.Format("{0} {1}", data["firstName"], data["lastName"]);
                                (window as MainWindow).DataContext = userInfo;
                            }
                        }
                        res["FirstName"] = data["firstName"];
                        res["LastName"] = data["lastName"];
                        res["StatusCode"] = data["statusCode"];
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
                        string query = String.Format("UPDATE UserAccount " +
                            "SET FirstName = '{0}', LastName = '{1}', StatusCode='{2}'{3} " +
                            "WHERE UserAccountId = '{4}'", data["firstName"], data["lastName"], data["statusCode"],
                            String.IsNullOrWhiteSpace(data["password"]) ? "" : ", UserPassword = '" + data["password"] + "'",
                            Application.Current.Resources["UserAccountId"].ToString());
                        SqlCommand cmd = new SqlCommand(query, con);
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

        private static async Task<string> getCommitUser(string guid)
        {
            string result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = String.Format("SELECT FirstName, LastName FROM UserAccount WHERE UserAccountId = '{0}'", guid);
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        using(SqlDataReader reader = cmd.ExecuteReader()){
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    return String.Format("{0} {1}", reader["FirstName"].ToString(), reader["LastName"].ToString());
                                }
                            } else
                            {
                                return null;
                            }
                        }
                        con.Close();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR");
                    return null;
                }
            });
            return result;
        }
    }
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }
        public override string ToString()
        {
            return Text;
        }
    }
}
