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
using System.IO;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for EditAirportTranslationWindow.xaml
    /// </summary>
    public partial class EditAirportTranslationWindow : Window
    {
        bool isStartUp = true;
        private string selectedAirportTransId = String.Empty;
        DBConnector dbCon = new DBConnector();
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        public EditAirportTranslationWindow()
        {
            InitializeComponent();
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            if(await saveNameInLanguage())
            {
                MessageBox.Show("Update Airport Translation Successfully", "SUCCESS");
            } else
            {
                MessageBox.Show("Error Occurred While Update Airport Translation", "ERROR");
            }
            saveBtn.IsEnabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FetchAirportList();
        }

        private async void FetchAirportList()
        {
            string query = "SELECT DISTINCT AirportCode, AirportName FROM AirportReference WHERE AirportCode IN (SELECT DISTINCT AirportCode FROM AirportTranslation) ORDER BY AirportCode";
            DataList airportList = (DataList)(await dbCon.customQuery(SQLStatementType.SELECT_LIST, query));
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
            string query = String.Format("SELECT LanguageCode, LanguageName FROM LanguageReference WHERE LanguageCode IN (SELECT LanguageCode FROM AirportTranslation WHERE AirportCode = '{0}') ORDER BY LanguageCode", airportCode);
            DataList languageList = (DataList)await dbCon.customQuery(SQLStatementType.SELECT_LIST, query);
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
                DisplayAirportTranslationInfo();
            }
            else if (!languageList.HasData && languageList.Error == ERROR.NoError)
            {
                MessageBox.Show("No more Language Reference to update to this Airport", "WARNING");
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
                    "AirportName", nameInLanguageTextBox.Text,
                    "CommitBy", Application.Current.Resources["UserAccountId"],
                    "CommitDateTime", DateTime.Now
                );
            return await dbCon.updateDataRow("AirportTranslation", data, new DataRow("AirportTranslationId", selectedAirportTransId));
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
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            nameInTextBlock.Text = LanguageComboBox.SelectedValue != null ? LanguageComboBox.SelectedValue.ToString() : String.Empty;
            if(LanguageComboBox.SelectedValue != null)
            {
                DisplayAirportTranslationInfo();

                nameInTextBlock.Visibility = Visibility.Visible;
                nameInTitleTextBlock.Visibility = Visibility.Visible;
                nameInLanguageTextBox.Visibility = Visibility.Visible;

                commitByTextBlock.Visibility = Visibility.Visible;
                commitByTextBlockValue.Visibility = Visibility.Visible;

                commitDateTimeTextBlock.Visibility = Visibility.Visible;
                commitDateTimeTextBlockValue.Visibility = Visibility.Visible;
            }
        }

        private async void DisplayAirportTranslationInfo()
        {
            string airportCode = airportNameComboBox.SelectedValue.ToString();
            string languageCode = LanguageComboBox.SelectedValue.ToString();

            DataRow airportTranslation = await dbCon.getDataRow("AirportTranslation", new DataRow("AirportCode", airportCode, "LanguageCode", languageCode));
            if(airportTranslation.HasData && airportTranslation.Error == ERROR.NoError)
            {
                nameInLanguageTextBox.Text = airportTranslation.Get("AirportName").ToString();
                DataRow commitByUser = await dbCon.getDataRow("UserAccount", new DataRow("UserAccountId", airportTranslation.Get("CommitBy")));
                if(commitByUser.HasData && commitByUser.Error == ERROR.NoError)
                {
                    commitByTextBlockValue.Text = String.Format("{0} {1}", commitByUser.Get("FirstName").ToString(), commitByUser.Get("LastName").ToString());
                }
                commitDateTimeTextBlockValue.Text = airportTranslation.Get("CommitDateTime").ToString();

                selectedAirportTransId = airportTranslation.Get("AirportTranslationId").ToString();
            } else
            {
                DialogResult = false;
                MessageBox.Show("Error occurred while fetching Airport Translation Info", "ERROR");
                this.Close();
            }
        }
    }
}
