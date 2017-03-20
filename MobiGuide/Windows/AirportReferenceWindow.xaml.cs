using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DatabaseConnector;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for AirportReferenceWindow.xaml
    /// </summary>
    public partial class AirportReferenceWindow : Window
    {
        DBConnector dbCon = new DBConnector();
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        public AirportReferenceWindow()
        {
            InitializeComponent();

            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Any", Value = "" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Inactive", Value = "I" });
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }
        private async void DoSearch()
        {
            string airportCode = airportCodeTextBox.Text;
            string airportName = airportNameTextBox.Text;
            string status = statusComboBox.SelectedValue.ToString();

            DataList list = await dbCon.GetDataList("AirportReference", null,
                String.Format("WHERE AirportCode LIKE '%{0}%' AND AirportName LIKE '%{1}%' AND StatusCode LIKE '%{2}%' COLLATE SQL_Latin1_General_CP1_CI_AS ORDER BY AirportCode",
                airportCode, airportName, status));
            if (list.HasData && list.Error == ERROR.NoError)
            {
                List<AirportReference> _itemSource = new List<AirportReference>();
                foreach (DataRow row in list)
                {
                    AirportReference airportRef = new AirportReference(
                        row.Get("AirportCode").ToString(),
                        row.Get("AirportName").ToString(),
                        row.Get("StatusCode").ToString()
                    );
                    _itemSource.Add(airportRef);
                }
                airportDataGrid.ItemsSource = _itemSource;
            }
            else
            {
                airportDataGrid.ItemsSource = null;
                airportDataGrid.SelectedIndex = -1;
            }
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            NewEditAirportReferenceWindow newAirportReferenceWindow = new NewEditAirportReferenceWindow();
            newAirportReferenceWindow.ShowDialog();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            int indexOfSelectedItem = airportDataGrid.SelectedIndex;
            string airportCode = (airportDataGrid.SelectedItem as AirportReference).AirportCode;
            NewEditAirportReferenceWindow editAirportReferenceWindow = new NewEditAirportReferenceWindow(airportCode);
            editAirportReferenceWindow.ShowDialog();
            if (editAirportReferenceWindow.DialogResult.HasValue && editAirportReferenceWindow.DialogResult.Value)
            {
                DoSearch();
            }
        }

        private void airportDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedIndex != -1) editBtn.IsEnabled = true;
            else editBtn.IsEnabled = false;
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            airportCodeTextBox.Text = String.Empty;
            airportNameTextBox.Text = String.Empty;
            statusComboBox.SelectedIndex = 0;
            airportDataGrid.ItemsSource = null;
            airportDataGrid.SelectedIndex = -1;
        }

        private void airportDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int indexOfSelectedItem = (sender as DataGrid).SelectedIndex;
            string airportCode = ((sender as DataGrid).SelectedItem as AirportReference).AirportCode;
            NewEditAirportReferenceWindow editAirportReferenceWindow = new NewEditAirportReferenceWindow(airportCode);
            editAirportReferenceWindow.ShowDialog();
            if (editAirportReferenceWindow.DialogResult.HasValue && editAirportReferenceWindow.DialogResult.Value) {
                DoSearch();
            }
        }
    }
}
