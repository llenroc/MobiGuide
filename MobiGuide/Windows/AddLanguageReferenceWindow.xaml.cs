using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DatabaseConnector;
using MobiGuide.Class;
using Properties;

namespace MobiGuide
{
    /// <summary>
    ///     Interaction logic for AddLanguageReferenceWindow.xaml
    /// </summary>
    public partial class AddLanguageReferenceWindow : Window
    {
        private readonly DBConnector dbCon = new DBConnector();

        public AddLanguageReferenceWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            statusComboBox.Items.Add(new CustomComboBoxItem {Text = "Active", Value = "A"});
            statusComboBox.Items.Add(new CustomComboBoxItem {Text = "Inactive", Value = "I"});
            languageCodeTextBox.Focus();
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            if (!IsLanguageCodeCorrect(languageCodeTextBox.Text))
            {
                MessageBox.Show(Messages.WARNING_WRONG_LANGUAGE_CODE, Captions.WARNING);
            }
            else if (string.IsNullOrWhiteSpace(languageNameTextBox.Text))
            {
                MessageBox.Show(Messages.WARNING_ENTER_LANGUAGE_NAME, Captions.WARNING);
            }
            else
            {
                if (await IsExistingLanguageCode(languageCodeTextBox.Text.ToUpper()))
                {
                    MessageBox.Show(Messages.WARNING_EXISTING_LANGUAGE_CODE, Captions.WARNING);
                }
                else
                {
                    var languageReferenceData = new DataRow(
                        "LanguageCode", languageCodeTextBox.Text.ToUpper(),
                        "LanguageName", languageNameTextBox.Text,
                        "StatusCode", statusComboBox.SelectedValue.ToString(),
                        "CommitBy", Application.Current.Resources["UserAccountId"],
                        "CommitDateTime", DateTime.Now);
                    if (await dbCon.CreateNewRow("LanguageReference", languageReferenceData, null))
                    {
                        DialogResult = true;
                        MessageBox.Show(Messages.SUCCESS_ADD_LANGUAGE_REF, Captions.SUCCESS);
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(Messages.ERROR_ADD_LANGUAGE_REF, Captions.ERROR);
                    }
                }
            }
            saveBtn.IsEnabled = true;
        }

        private bool IsLanguageCodeCorrect(string code)
        {
            if ((code.Length == 2 || code.Length == 3) && IsTextAllowed(code))
                return true;
            return false;
        }

        private async Task<bool> IsExistingLanguageCode(string code)
        {
            var result = await dbCon.GetDataRow("LanguageReference", new DataRow("LanguageCode", code));
            if (result.HasData && result.Error == ERROR.NoError) return true;
            if (result.Error == ERROR.HasError) return true;
            return false;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void languageCodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            var regex = new Regex("[a-zA-Z]"); //regex that matches disallowed text
            return regex.IsMatch(text);
        }
    }
}