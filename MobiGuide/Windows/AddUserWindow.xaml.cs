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
using CustomExtensions;
using DatabaseConnector;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        static ResourceDictionary res = Application.Current.Resources;
        DBConnector dbCon = new DBConnector();
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
                        DataRow data = new DataRow();
                        data.Set("UserLogon", uNameTxtBox.Text);
                        data.Set("UserPassword", pwdBox.Password);
                        data.Set("LastName", lastNameTxtBox.Text);
                        data.Set("FirstName", firstNameTxtBox.Text);
                        data.Set("StatusCode", (statusComboBox.SelectedItem as CustomComboBoxItem).Value.ToString());
                        data.Set("AirlineCode", res["AirlineCode"]);
                        data.Set("CommitBy", res["UserAccountId"]);
                        data.Set("CommitDateTime", DateTime.Now);
                        string cfmMsg = String.Format("Do you want to add new user?" + System.Environment.NewLine + System.Environment.NewLine +
                            "Username : {0}" + System.Environment.NewLine +
                            "First Name : {1}" + System.Environment.NewLine +
                            "Last Name : {2}" + System.Environment.NewLine +
                            "Password : {3}", uNameTxtBox.Text, firstNameTxtBox.Text, lastNameTxtBox.Text,
                            pwdBox.Password.Length == 0 ? "(unchanged)" : new string('*', pwdBox.Password.Length));
                        MessageBoxResult confirmResult = MessageBox.Show(cfmMsg, "Confirm?", MessageBoxButton.OKCancel);
                        if (confirmResult == MessageBoxResult.OK)
                        {
                            bool result = await dbCon.CreateNewRow("UserAccount", data, "UserAccountId");
                            if (result)
                            {
                                MessageBox.Show(String.Format("Add user [{0}] successfully.", data.Get("UserLogon")), "SUCCESS");
                                DialogResult = true;
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Cannot add new user at this time.", "ERROR");
                            }
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
            DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            displayInfo();
        }

        private async void displayInfo()
        {
            alNameTxtBlock.Text = (await dbCon.GetDataRow("AirlineReference", 
                new DataRow("AirlineCode", res["AirlineCode"]))).Get("AirlineName").ToString();

            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Inactive", Value = "I" });
            statusComboBox.SelectedIndex = 0;

            uNameTxtBox.Focus();
        }

        private async Task<uLogon> checkExistingULogon(string uName)
        {
            try
            {
                if ((await dbCon.GetDataRow("UserAccount", new DataRow("UserLogon", uName))).HasData)
                {
                    return uLogon.Exist;
                }
                else return uLogon.NoExist;
            } catch (Exception ex)
            {
                return uLogon.Error;
            }
        }
        private enum uLogon
        {
            Exist,
            NoExist,
            Error
        }
    }
}
