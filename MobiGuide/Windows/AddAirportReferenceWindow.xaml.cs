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
    /// Interaction logic for AddAirportReferenceWindow.xaml
    /// </summary>
    public partial class AddAirportReferenceWindow : Window
    {
        private readonly DBConnector dbCon = new DBConnector();

        public AddAirportReferenceWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Inactive", Value = "I" });
            airportCodeTextBox.Focus();
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            if (!IsAirportCodeCorrect(airportCodeTextBox.Text))
            {
                MessageBox.Show(Messages.WARNING_WRONG_AIRLINE_CODE, Captions.WARNING);
            }
            else if(string.IsNullOrWhiteSpace(airportNameTextBox.Text))
            {
                MessageBox.Show(Messages.ERROR_ENTER_AIRLINE_NAME, Captions.WARNING);
            } else
            {
                if (await IsExistingAirportCode(airportCodeTextBox.Text.ToUpper()))
                {
                    MessageBox.Show(Messages.ERROR_EXISTING_AIRLINE_CODE, Captions.WARNING);
                } else
                {
                    DataRow airportReferenceData = new DataRow(
                        "AirportCode", airportCodeTextBox.Text.ToUpper(),
                        "AirportName", airportNameTextBox.Text,
                        "StatusCode", statusComboBox.SelectedValue.ToString(),
                        "CommitBy", Application.Current.Resources["UserAccountId"],
                        "CommitDateTime", DateTime.Now);
                    if (await dbCon.CreateNewRow("AirportReference", airportReferenceData, null))
                    {
                        DialogResult = true;
                        MessageBox.Show(Messages.SUCCESS_ADD_AIRPORT_REF, Captions.SUCCESS);
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(Messages.ERROR_ADD_AIRPORT_REF, Captions.ERROR);
                    }
                }
            }
            saveBtn.IsEnabled = true;
        }

        private bool IsAirportCodeCorrect(string code)
        {
            if (code.Length == 3 && IsTextAllowed(code))
                return true;
            return false;
        }

        private async Task<bool> IsExistingAirportCode(string code)
        {
            DataRow result = await dbCon.GetDataRow("AirportReference", new DataRow("AirportCode", code));
            if (result.HasData && result.Error == ERROR.NoError) return true;
            if (result.Error == ERROR.HasError) return true;
            return false;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void airportCodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
