using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DatabaseConnector;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for AddLanguageReferenceWindow.xaml
    /// </summary>
    public partial class AddLanguageReferenceWindow : Window
    {
        DBConnector dbCon = new DBConnector();
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        public AddLanguageReferenceWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            statusComboBox.Items.Add(new ComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new ComboBoxItem { Text = "Inactive", Value = "I" });
            languageCodeTextBox.Focus();
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            if (!IsLanguageCodeCorrect(languageCodeTextBox.Text))
            {
                MessageBox.Show("Airline Code Must Contains 2-3 Characters", "WARNING");
            }
            else if(String.IsNullOrWhiteSpace(languageNameTextBox.Text))
            {
                MessageBox.Show("Please Enter Airline Name", "WARNING");
            } else
            {
                if (await IsExistingLanguageCode(languageCodeTextBox.Text.ToUpper()))
                {
                    MessageBox.Show("Language Code is Existing", "WARNING");
                } else
                {
                    DataRow languageReferenceData = new DataRow(
                        "LanguageCode", languageCodeTextBox.Text.ToUpper(),
                        "LanguageName", languageNameTextBox.Text,
                        "StatusCode", statusComboBox.SelectedValue.ToString(),
                        "CommitBy", Application.Current.Resources["UserAccountId"],
                        "CommitDateTime", DateTime.Now);
                    if (await dbCon.createNewRow("LanguageReference", languageReferenceData, null))
                    {
                        DialogResult = true;
                        MessageBox.Show("Add Language Reference Successfully", "SUCCESS");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Cannot Save Language Reference", "ERROR");
                    }
                }
            }
            saveBtn.IsEnabled = true;
        }

        private bool IsLanguageCodeCorrect(string code)
        {
            if ((code.Length == 2 || code.Length == 3) && IsTextAllowed(code))
                return true;
            else return false;
        }
        private async Task<bool> IsExistingLanguageCode(string code)
        {
            DataRow result = await dbCon.getDataRow("LanguageReference", new DataRow("LanguageCode", code));
            if (result.HasData && result.Error == ERROR.NoError) return true;
            else if (result.Error == ERROR.HasError) return true;
            else return false;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void languageCodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[a-zA-Z]"); //regex that matches disallowed text
            return regex.IsMatch(text);
        }
    }
}
