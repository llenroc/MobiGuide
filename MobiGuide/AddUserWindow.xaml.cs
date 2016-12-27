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
    /// Interaction logic for AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        static ResourceDictionary res = Application.Current.Resources;
        static string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
        public AddUserWindow()
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
            if (e.Key == Key.Enter)
            {
                saveBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            base.OnPreviewKeyDown(e);
        }
        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(firstNameTxtBox.Text) || String.IsNullOrWhiteSpace(lastNameTxtBox.Text) || String.IsNullOrWhiteSpace(uNameTxtBox.Text))
            {
                MessageBox.Show("Please fill every fields before save!", "WARNING");
            }
            else if (String.IsNullOrWhiteSpace(pwdBox.Password) || String.IsNullOrWhiteSpace(cfmPwdBox.Password))
            {
                MessageBox.Show("Please fill both Password and Confirm Password fields", "WARNING");
            }
            else if (!pwdBox.Password.Equals(cfmPwdBox.Password))
            {
                MessageBox.Show("Please match your password", "WARNING");
            }
            else
            {
                uLogon isExistingUName = await checkExistingULogon(uNameTxtBox.Text);
                switch (isExistingUName)
                {
                    case uLogon.Exist: 
                        MessageBox.Show("This Username already exist. Please use another one.", "WARNING");
                        break;
                    case uLogon.Error:
                        MessageBox.Show("Unexpected error occurred! Please contact administrator.", "ERROR");
                        break;
                    case uLogon.NoExist:
                        Dictionary<string, string> data = new Dictionary<string, string>();
                        data.Add("UserLogon", uNameTxtBox.Text);
                        data.Add("UserPassword", pwdBox.Password);
                        data.Add("LastName", lastNameTxtBox.Text);
                        data.Add("FirstName", firstNameTxtBox.Text);
                        bool result = await createNewUser(data);
                        if (result)
                        {
                            this.Hide();
                            MessageBox.Show(String.Format("Add user [{0}] successfully.", data["UserLogon"]), "SUCCESS");
                            this.Close();
                        } else
                        {
                            MessageBox.Show("Cannot add new user at this time.", "ERROR");
                        }
                        break;
                    default:
                        MessageBox.Show("Cannot add new user at this time.", "ERROR");
                        break;
                }
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            alNameTxtBlock.Text = res["AirlineName"].ToString();
            uNameTxtBox.Focus();
        }

        private static async Task<uLogon> checkExistingULogon(string uName)
        {
            uLogon result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = String.Format("SELECT UserLogon FROM UserAccount WHERE UserLogon = '{0}'", uName);
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            bool hasRow = reader.HasRows;
                            con.Close();
                            if (hasRow) return uLogon.Exist;
                            else return uLogon.NoExist;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Result");
                    return uLogon.Error;
                }
            });
            return result;
        }
        private static async Task<bool> createNewUser(Dictionary<string, string> data)
        {
            bool result = await Task.Run(() =>
            {
                try
                {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = String.Format("DECLARE @id uniqueidentifier " +
                        "SET @id = NEWID() " +
                        "INSERT INTO UserAccount VALUES (@id,'{0}','{1}','{2}','{3}','{4}',NULL,'{5}',GETDATE())",
                        res["AirlineCode"], data["UserLogon"], data["UserPassword"], data["LastName"], data["FirstName"], res["UserAccountId"]);
                        con.Open();
                        SqlCommand cmd = new SqlCommand(query, con);
                        int row = cmd.ExecuteNonQuery();
                        if (row > 0) return true;
                        else return false;
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
        private enum uLogon
        {
            Exist,
            NoExist,
            Error
        }
    }
}
