using DatabaseConnector;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for EditAirportReferenceWindow.xaml
    /// </summary>
    public partial class EditAirportReferenceWindow : Window
    {
        DBConnector dbCon = new DBConnector();
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        public EditAirportReferenceWindow()
        {
            InitializeComponent();
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            DataRow data = new DataRow(
                "AirportName", airportNameTextBox.Text,
                "StatusCode", statusComboBox.SelectedValue,
                "CommitBy", Application.Current.Resources["UserAccountId"],
                "CommitDateTime", DateTime.Now
                );
            if(await SaveEditAirportReference(data, new DataRow("AirportCode", airportCodeComboBox.SelectedValue))){
                MessageBox.Show(String.Format("Update [{0}] Airport Reference Successfully", airportCodeComboBox.SelectedValue), "SUCCESS");
                GetAirportReferenceData(airportCodeComboBox.SelectedValue.ToString());
            } else
            {
                MessageBox.Show(String.Format("Failed to Update [{0}] Airport Reference", airportCodeComboBox.SelectedValue), "ERROR");
            }
            saveBtn.IsEnabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Inactive", Value = "I" });

            FetchAirportList();
        }

        private async void FetchAirportList()
        {
            airportCodeComboBox.Items.Clear();
            DataList list = await dbCon.GetDataList("AirportReference", null, "ORDER BY AirportCode");

            airportCodeComboBox.Items.Add(new CustomComboBoxItem { Text = "---", Value = "NOSEL" });
            if(list.Error == ERROR.NoError && list.HasData)
            {
                foreach(DataRow row in list)
                {
                    airportCodeComboBox.Items.Add(new CustomComboBoxItem { Text = row.Get("AirportCode").ToString(), Value = row.Get("AirportCode").ToString() });
                }
                airportCodeComboBox.SelectionChanged += RemoveNotSelectOptionItem;
                airportCodeComboBox.SelectionChanged += AirportCodeComboBox_SelectionChanged;

            } else
            {
                MessageBox.Show("Cannot Get Airport Code List", "ERROR");
            }
        }

        private void AirportCodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetAirportReferenceData(airportCodeComboBox.SelectedValue.ToString());
        }

        private async void GetAirportReferenceData(string airportCode)
        {
            DataRow airportRef = await dbCon.GetDataRow("AirportReference", new DataRow("AirportCode", airportCode));
            if(airportRef.HasData && airportRef.Error == ERROR.NoError)
            {
                airportNameTextBox.Text = airportRef.Get("AirportName") != DBNull.Value ? airportRef.Get("AirportName").ToString() : "NULL";
                statusComboBox.SelectedValue = airportRef.Get("StatusCode") != DBNull.Value ? airportRef.Get("StatusCode") : "A";
                commitDateTimeTextBlockValue.Text = airportRef.Get("CommitDateTime") != DBNull.Value ? airportRef.Get("CommitDateTime").ToString() : "NULL";
                commitByTextBlockValue.Text = await dbCon.GetFullNameFromUid(airportRef.Get("CommitBy").ToString());
            }
        }

        private async Task<bool> SaveEditAirportReference(DataRow data, DataRow conditions)
        {
            return await dbCon.UpdateDataRow("AirportReference", data, conditions);
            
        }

        private void RemoveNotSelectOptionItem(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).Items.Count > 0) (sender as ComboBox).Items.RemoveAt(0);
            (sender as ComboBox).SelectionChanged -= RemoveNotSelectOptionItem;
            airportNameTextBlock.Visibility = Visibility.Visible;
            airportNameTextBox.Visibility = Visibility.Visible;
            statusTextBlock.Visibility = Visibility.Visible;
            statusComboBox.Visibility = Visibility.Visible;
            commitByTextBlock.Visibility = Visibility.Visible;
            commitByTextBlockValue.Visibility = Visibility.Visible;
            commitDateTimeTextBlock.Visibility = Visibility.Visible;
            commitDateTimeTextBlockValue.Visibility = Visibility.Visible;
        }

        private void airportNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(airportNameTextBox.Text)) saveBtn.IsEnabled = true;
            else saveBtn.IsEnabled = false;
        }
    }
}
