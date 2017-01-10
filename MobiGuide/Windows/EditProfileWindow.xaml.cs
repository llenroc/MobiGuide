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

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for EditProfileWindow.xaml
    /// </summary>
    public partial class EditProfileWindow : Window
    {
        ResourceDictionary res = Application.Current.Resources;
        DBConnector dbCon = new DBConnector();
        public EditProfileWindow()
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
            DataRow userInfo = await dbCon.getDataRow("UserAccount", new DataRow("UserAccountId", res["UserAccountId"]));

            uNameTxtBlock.Text = userInfo.Get("UserLogon").ToString();
            firstNameTxtBox.Text = userInfo.Get("FirstName").ToString();
            lastNameTxtBox.Text = userInfo.Get("LastName").ToString();
            commitDateTxtBlock.Text = userInfo.Get("CommitDateTime").ToString();

            alNameTxtBlock.Text = (await dbCon.getDataRow("AirlineReference",
                new DataRow("AirlineCode", res["AirlineCode"]))).Get("AirlineName").ToString();
            DataRow commitByUser = await dbCon.getDataRow("UserAccount", new DataRow("UserAccountId", userInfo.Get("CommitBy")));
            commitByTxtBlock.Text = String.Format("{0} {1}", commitByUser.Get("FirstName").ToString(), commitByUser.Get("LastName").ToString());

            statusComboBox.Items.Add(new ComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new ComboBoxItem { Text = "Inactive", Value = "I" });

            for (int i = 0; i < statusComboBox.Items.Count; i++)
            {
                if (userInfo.Get("StatusCode").ToString().Equals((statusComboBox.Items[i] as ComboBoxItem).Value))
                {
                    statusComboBox.SelectedIndex = i;
                    break;
                }
            }
            firstNameTxtBox.Focus();
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!newPwdBox.Password.Equals(cfmPwdBox.Password))
            {
                MessageBox.Show("Please match your new password", "ERROR");
            }
            else
            {
                string cfmMsg = String.Format("Do you want to save changes?" + System.Environment.NewLine + System.Environment.NewLine +
                    "First Name : {0}" + System.Environment.NewLine +
                    "Last Name : {1}" + System.Environment.NewLine +
                    "Password : {2}", firstNameTxtBox.Text, lastNameTxtBox.Text,
                    newPwdBox.Password.Length == 0 ? "(unchanged)" : new string('*', newPwdBox.Password.Length));
                MessageBoxResult result = MessageBox.Show(cfmMsg, "Confirm?", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    DataRow data = new DataRow();
                    data.Set("FirstName", firstNameTxtBox.Text);
                    data.Set("LastName", lastNameTxtBox.Text);
                    data.Set("StatusCode", (statusComboBox.SelectedItem as ComboBoxItem).Value.ToString());
                    if(newPwdBox.Password.Length != 0) data.Set("UserPassword", cfmPwdBox.Password);

                    if (await dbCon.updateDataRow("UserAccount", data, new DataRow("UserAccountId", res["UserAccountId"])))
                    {
                        MessageBox.Show("Update Profile Succussfully.", "SUCCESS");
                        DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update profile.", "ERROR");
                        DialogResult = false;
                        this.Close();
                    }
                }
            }
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
