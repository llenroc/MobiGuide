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
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for AddAirportReferenceWindow.xaml
    /// </summary>
    public partial class AddAirportReferenceWindow : Window
    {
        DBConnector dbCon = new DBConnector();
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        public AddAirportReferenceWindow()
        {
            InitializeComponent();
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
                MessageBox.Show("Airline Code Must Contains 3 Characters", "WARNING");
            }
            else if(String.IsNullOrWhiteSpace(airportNameTextBox.Text))
            {
                MessageBox.Show("Please Enter Airline Name", "WARNING");
            } else
            {
                if (await IsExistingAirportCode(airportCodeTextBox.Text.ToUpper()))
                {
                    MessageBox.Show("Airport Code is Existing", "WARNING");
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
                        MessageBox.Show("Add Airport Reference Successfully", "SUCCESS");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Cannot Save Airport Reference", "ERROR");
                    }
                }
            }
            saveBtn.IsEnabled = true;
        }

        private bool IsAirportCodeCorrect(string code)
        {
            if (code.Length == 3 && IsTextAllowed(code))
                return true;
            else return false;
        }
        private async Task<bool> IsExistingAirportCode(string code)
        {
            DataRow result = await dbCon.GetDataRow("AirportReference", new DataRow("AirportCode", code));
            if (result.HasData && result.Error == ERROR.NoError) return true;
            else if (result.Error == ERROR.HasError) return true;
            else return false;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
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
