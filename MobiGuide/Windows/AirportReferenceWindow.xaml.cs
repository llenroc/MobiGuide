using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            statusComboBox.Items.Add(new ComboBoxItem { Text = "Any", Value = "" });
            statusComboBox.Items.Add(new ComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new ComboBoxItem { Text = "Inactive", Value = "I" });
        }

        private async void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            string airportCode = airportCodeTextBox.Text;
            string airportName = airportNameTextBox.Text;
            string status = statusComboBox.SelectedValue.ToString();

            DataList list = await dbCon.getDataList("AirportReference", null, 
                String.Format("WHERE AirportCode LIKE '%{0}%' AND AirportName LIKE '%{1}%' AND StatusCode LIKE '%{2}%' COLLATE SQL_Latin1_General_CP1_CI_AS ORDER BY AirportCode", 
                airportCode, airportName, status));
            if(list.HasData && list.Error == ERROR.NoError)
            {
                List<AirportReference> _itemSource = new List<AirportReference>();
                foreach(DataRow row in list)
                {
                    AirportReference airportRef = new AirportReference(
                        row.Get("AirportCode").ToString(),
                        row.Get("AirportName").ToString(),
                        row.Get("StatusCode").ToString()
                    );
                    _itemSource.Add(airportRef);
                }
                airportDataGrid.ItemsSource = _itemSource;
            } else
            {
                airportDataGrid.ItemsSource = null;
                airportDataGrid.SelectedIndex = -1;
            }
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            NewEditAirportReferenceWindow newAirportReferenceWindow = new NewEditAirportReferenceWindow(STATUS.NEW);
            newAirportReferenceWindow.ShowDialog();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            NewEditAirportReferenceWindow editAirportReferenceWindow = new NewEditAirportReferenceWindow(STATUS.EDIT);
            editAirportReferenceWindow.ShowDialog();
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
    }
    public class AirportReference
    {
        private string _airportCode;
        private string _airportName;
        private string _status;
        public AirportReference() : this(String.Empty, String.Empty, String.Empty)
        {
        }
        public AirportReference(string airportCode, string airportName, string statusCode)
        {
            _airportCode = airportCode;
            _airportName = airportName;
            switch (statusCode)
            {
                case "A":
                    _status = "Active";
                    break;
                case "I":
                    _status = "Inactive";
                    break;
            }
        }
        public string AirportCode
        {
            get { return _airportCode; }
            set { _airportCode = value; }
        }
        public string AirportName
        {
            get { return _airportName; }
            set { _airportName = value; }
        }

        public string SeletedLanguageCode { get; internal set; }

        public string Status
        {
            get { return _status; }
            set
            {
                switch (value)
                {
                    case "A":
                        _status = "Active";
                        break;
                    case "I":
                        _status = "Inactive";
                        break;
                }
            }
        }

    }
}
