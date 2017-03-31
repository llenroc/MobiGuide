using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DatabaseConnector;
using MobiGuide.Class;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for EditProfileWindow.xaml
    /// </summary>
    public partial class EditProfileWindow : Window
    {
        private readonly DBConnector dbCon = new DBConnector();
        private readonly ResourceDictionary res = Application.Current.Resources;

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
                e.Handled = true;
            if (e.Key == Key.Enter)
                saveBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            base.OnPreviewKeyDown(e);
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            displayInfo();
        }

        private async void displayInfo()
        {
            DataRow userInfo = await dbCon.GetDataRow("UserAccount", new DataRow("UserAccountId", res["UserAccountId"]));

            uNameTxtBlock.Text = userInfo.Get("UserLogon").ToString();
            firstNameTxtBox.Text = userInfo.Get("FirstName").ToString();
            lastNameTxtBox.Text = userInfo.Get("LastName").ToString();
            commitDateTxtBlock.Text = userInfo.Get("CommitDateTime").ToString();

            alNameTxtBlock.Text = (await dbCon.GetDataRow("AirlineReference",
                new DataRow("AirlineCode", res["AirlineCode"]))).Get("AirlineName").ToString();
            commitByTxtBlock.Text = await dbCon.GetFullNameFromUid(userInfo.Get("CommitBy").ToString());

            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Inactive", Value = "I" });

            for (int i = 0; i < statusComboBox.Items.Count; i++)
                if (userInfo.Get("StatusCode").ToString().Equals((statusComboBox.Items[i] as CustomComboBoxItem).Value))
                {
                    statusComboBox.SelectedIndex = i;
                    break;
                }
            firstNameTxtBox.Focus();
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!newPwdBox.Password.Equals(cfmPwdBox.Password))
            {
                MessageBox.Show(Messages.WARNING_PASS_NOT_MATCH, Captions.WARNING);
            }
            else
            {
                string cfmMsg = string.Format("Do you want to save changes?" + Environment.NewLine + Environment.NewLine +
                    "First Name : {0}" + Environment.NewLine +
                    "Last Name : {1}" + Environment.NewLine +
                    "Password : {2}", firstNameTxtBox.Text, lastNameTxtBox.Text,
                    newPwdBox.Password.Length == 0 ? "(unchanged)" : new string('*', newPwdBox.Password.Length));
                MessageBoxResult result = MessageBox.Show(cfmMsg, Captions.CONFIRM_QUESTION, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    DataRow data = new DataRow();
                    data.Set("FirstName", firstNameTxtBox.Text);
                    data.Set("LastName", lastNameTxtBox.Text);
                    data.Set("StatusCode", (statusComboBox.SelectedItem as CustomComboBoxItem).Value.ToString());
                    if(newPwdBox.Password.Length != 0) data.Set("UserPassword", cfmPwdBox.Password);

                    if (await dbCon.UpdateDataRow("UserAccount", data, new DataRow("UserAccountId", res["UserAccountId"])))
                    {
                        MessageBox.Show(Messages.SUCCESS_UPDATE_UPROFILE, Captions.SUCCESS);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(Messages.ERROR_UPDATE_UPROFILE, Captions.ERROR);
                        DialogResult = false;
                        Close();
                    }
                }
            }
        }
    }
}
