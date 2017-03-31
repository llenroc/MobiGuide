using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DatabaseConnector;
using MobiGuide.Class;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        private static readonly ResourceDictionary res = Application.Current.Resources;
        private readonly DBConnector dbCon = new DBConnector();

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
                e.Handled = true;
            base.OnPreviewKeyDown(e);
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(firstNameTxtBox.Text) || string.IsNullOrWhiteSpace(lastNameTxtBox.Text) || string.IsNullOrWhiteSpace(uNameTxtBox.Text))
            {
                MessageBox.Show(Messages.WARNING_NOT_FILLED_FIELDS, Captions.WARNING);
            }
            else if (string.IsNullOrWhiteSpace(pwdBox.Password) || string.IsNullOrWhiteSpace(cfmPwdBox.Password))
            {
                MessageBox.Show(Messages.WARNING_PASS_NOT_FILLED, Captions.WARNING);
            }
            else if (!pwdBox.Password.Equals(cfmPwdBox.Password))
            {
                MessageBox.Show(Messages.WARNING_PASS_NOT_MATCH, Captions.WARNING);
            }
            else
            {
                uLogon isExistingUName = await checkExistingULogon(uNameTxtBox.Text);
                switch (isExistingUName)
                {
                    case uLogon.Exist: 
                        MessageBox.Show(Messages.WARNING_EXISTING_UNAME, Captions.WARNING);
                        break;
                    case uLogon.Error:
                        MessageBox.Show(Messages.ERROR_UNKNOWN, Captions.ERROR);
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
                        string cfmMsg = string.Format("Do you want to add new user?" + Environment.NewLine + Environment.NewLine +
                            "Username : {0}" + Environment.NewLine +
                            "First Name : {1}" + Environment.NewLine +
                            "Last Name : {2}" + Environment.NewLine +
                            "Password : {3}", uNameTxtBox.Text, firstNameTxtBox.Text, lastNameTxtBox.Text,
                            pwdBox.Password.Length == 0 ? "(unchanged)" : new string('*', pwdBox.Password.Length));
                        MessageBoxResult confirmResult = MessageBox.Show(cfmMsg, Captions.CONFIRM_QUESTION, MessageBoxButton.OKCancel);
                        if (confirmResult == MessageBoxResult.OK)
                        {
                            bool result = await dbCon.CreateNewRow("UserAccount", data, "UserAccountId");
                            if (result)
                            {
                                MessageBox.Show(string.Format(Messages.SUCCESS_ADD_USER(data.Get("UserLogon").ToString())), Captions.SUCCESS);
                                DialogResult = true;
                                Close();
                            }
                            else
                            {
                                MessageBox.Show(Messages.ERROR_ADD_USER, Captions.ERROR);
                            }
                        }
                        break;
                    default:
                        MessageBox.Show(Messages.ERROR_ADD_USER, Captions.ERROR);
                        break;
                }
            }
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
                    return uLogon.Exist;
                return uLogon.NoExist;
            } catch (Exception)
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
