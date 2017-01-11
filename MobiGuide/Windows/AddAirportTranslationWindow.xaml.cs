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
using System.Diagnostics;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for AddAirportTranslationWindow.xaml
    /// </summary>
    public partial class AddAirportTranslationWindow : Window
    {
        bool isStartUp = true;
        DBConnector dbCon = new DBConnector();
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        public AddAirportTranslationWindow()
        {
            InitializeComponent();
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            if(await saveNameInLanguage())
            {
                DialogResult = true;
                MessageBox.Show("Add Airport Translation Successfully", "SUCCESS");
                this.Close();
            } else
            {
                MessageBox.Show("Error Occurred While Add Airport Translation", "ERROR");
            }
            saveBtn.IsEnabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FetchAirportList();
        }

        private async void FetchAirportList()
        {
            DataList airportList = await dbCon.getDataList("AirportReference", null, "ORDER BY AirportCode");
            if (airportList.HasData && airportList.Error == ERROR.NoError)
            {
                foreach (DataRow airport in airportList)
                {
                    airportNameComboBox.Items.Add(new ComboBoxItem
                    {
                        Text = String.Format("{0} - {1}", airport.Get("AirportCode"), airport.Get("AirportName")),
                        Value = airport.Get("AirportCode")
                    });
                }
                airportNameComboBox.SelectionChanged += RemoveNotSelectOptionItem;
            } else if (!airportList.HasData && airportList.Error == ERROR.NoError)
            {
                MessageBox.Show("Airport Reference Not Found", "WARNING");
                this.Close();
            } else
            {
                MessageBox.Show("Error Occurred While Fetching Airport Reference", "ERROR");
                this.Close();
            }
        }

        private async void FetchLanguageList(string airportCode)
        {
            LanguageComboBox.Items.Clear();
            string addQuery = String.Empty;
            DataList airportTransList = await dbCon.getDataList("AirportTranslation", new DataRow("AirportCode", airportCode));
            if(airportTransList.HasData && airportTransList.Error == ERROR.NoError)
            {
                addQuery += "WHERE LanguageCode NOT IN (";
                for(int i = 0; i < airportTransList.Count; i++)
                {
                    addQuery += String.Format("'{0}'", airportTransList.GetListAt(i).Get("LanguageCode").ToString());
                    if (i < airportTransList.Count - 1) addQuery += ",";
                }
                addQuery += ") ";
            }
            DataList languageList = await dbCon.getDataList("LanguageReference", null, String.Format("{0}ORDER BY LanguageCode", addQuery));
            if (languageList.HasData && languageList.Error == ERROR.NoError)
            {
                foreach (DataRow language in languageList)
                {
                    LanguageComboBox.Items.Add(new ComboBoxItem
                    {
                        Text = String.Format("{0} - {1}", language.Get("LanguageCode"), language.Get("LanguageName")),
                        Value = language.Get("LanguageCode")
                    });
                }
                LanguageComboBox.SelectedIndex = 0;
            }
            else if (!languageList.HasData && languageList.Error == ERROR.NoError)
            {
                MessageBox.Show("No more Language Reference to add to this Airport", "WARNING");
                this.Close();
            }
            else
            {
                MessageBox.Show("Error Occurred While Fetching Language Reference", "ERROR");
                this.Close();
            }
        }

        private void RemoveNotSelectOptionItem(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).Items.Count > 0) (sender as ComboBox).Items.RemoveAt(0);
            (sender as ComboBox).SelectionChanged -= RemoveNotSelectOptionItem;
        }

        private void nameInLanguageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(nameInLanguageTextBox.Text)) saveBtn.IsEnabled = true;
            else saveBtn.IsEnabled = false;
        }

        private async Task<bool> saveNameInLanguage()
        {
            DataRow data = new DataRow(
                    "AirportCode", airportNameComboBox.SelectedValue,
                    "LanguageCode", LanguageComboBox.SelectedValue,
                    "AirportName", nameInLanguageTextBox.Text,
                    "CommitBy", Application.Current.Resources["UserAccountId"],
                    "CommitDateTime", DateTime.Now
                );
            return await dbCon.createNewRow("AirportTranslation", data, "AirportTranslationId");
        }

        private void airportNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isStartUp)
            {
                isStartUp = false;
            } else
            {
                FetchLanguageList(airportNameComboBox.SelectedValue.ToString());

                languageTextBlock.Visibility = Visibility.Visible;
                LanguageComboBox.Visibility = Visibility.Visible;

                nameInTextBlock.Visibility = Visibility.Visible;
                nameInTitleTextBlock.Visibility = Visibility.Visible;
                nameInLanguageTextBox.Visibility = Visibility.Visible;
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            nameInTextBlock.Text = LanguageComboBox.SelectedValue != null ? LanguageComboBox.SelectedValue.ToString() : String.Empty;
        }
    }
}
