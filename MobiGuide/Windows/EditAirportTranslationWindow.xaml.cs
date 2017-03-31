using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DatabaseConnector;
using MobiGuide.Class;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for EditAirportTranslationWindow.xaml
    /// </summary>
    public partial class EditAirportTranslationWindow : Window
    {
        private readonly DBConnector dbCon = new DBConnector();
        private bool isStartUp = true;
        private string selectedAirportTransId = string.Empty;

        public EditAirportTranslationWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            if(await saveNameInLanguage())
                MessageBox.Show(Messages.SUCCESS_UPDATE_AIRPORT_TRANSLATION, Captions.SUCCESS);
            else
                MessageBox.Show(Messages.ERROR_UPDATE_AIRPORT_TRANSLATION, Captions.ERROR);
            saveBtn.IsEnabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FetchAirportList();
        }

        private async void FetchAirportList()
        {
            string query = "SELECT DISTINCT AirportCode, AirportName FROM AirportReference WHERE AirportCode IN (SELECT DISTINCT AirportCode FROM AirportTranslation) ORDER BY AirportCode";
            DataList airportList = (DataList)await dbCon.CustomQuery(SQLStatementType.SELECT_LIST, query);
            if (airportList.HasData && airportList.Error == ERROR.NoError)
            {
                foreach (DataRow airport in airportList)
                    airportNameComboBox.Items.Add(new CustomComboBoxItem
                    {
                        Text = string.Format("{0} - {1}", airport.Get("AirportCode"), airport.Get("AirportName")),
                        Value = airport.Get("AirportCode")
                    });
                airportNameComboBox.SelectionChanged += RemoveNotSelectOptionItem;
            } else if (!airportList.HasData && airportList.Error == ERROR.NoError)
            {
                MessageBox.Show(Messages.WARNING_AIRPORT_REF_NOT_FOUND, Captions.WARNING);
                Close();
            } else
            {
                MessageBox.Show(Messages.ERROR_GET_AIRPORT_REF, Captions.ERROR);
                Close();
            }
        }

        private async void FetchLanguageList(string airportCode)
        {
            LanguageComboBox.Items.Clear();
            string query = string.Format("SELECT LanguageCode, LanguageName FROM LanguageReference WHERE LanguageCode IN (SELECT LanguageCode FROM AirportTranslation WHERE AirportCode = '{0}') ORDER BY LanguageCode", airportCode);
            DataList languageList = (DataList)await dbCon.CustomQuery(SQLStatementType.SELECT_LIST, query);
            if (languageList.HasData && languageList.Error == ERROR.NoError)
            {
                foreach (DataRow language in languageList)
                    LanguageComboBox.Items.Add(new CustomComboBoxItem
                    {
                        Text = string.Format("{0} - {1}", language.Get("LanguageCode"), language.Get("LanguageName")),
                        Value = language.Get("LanguageCode")
                    });
                LanguageComboBox.SelectedIndex = 0;
                DisplayAirportTranslationInfo();
            }
            else if (!languageList.HasData && languageList.Error == ERROR.NoError)
            {
                MessageBox.Show(Messages.WARNING_NO_MORE_LANGUAGE_REF_ADD, Captions.WARNING);
                Close();
            }
            else
            {
                MessageBox.Show(Messages.ERROR_GET_LANGUAGE_REF, Captions.ERROR);
                Close();
            }
        }

        private void RemoveNotSelectOptionItem(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).Items.Count > 0) (sender as ComboBox).Items.RemoveAt(0);
            (sender as ComboBox).SelectionChanged -= RemoveNotSelectOptionItem;
        }

        private void nameInLanguageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(nameInLanguageTextBox.Text)) saveBtn.IsEnabled = true;
            else saveBtn.IsEnabled = false;
        }

        private async Task<bool> saveNameInLanguage()
        {
            DataRow data = new DataRow(
                    "AirportName", nameInLanguageTextBox.Text,
                    "CommitBy", Application.Current.Resources["UserAccountId"],
                    "CommitDateTime", DateTime.Now
                );
            return await dbCon.UpdateDataRow("AirportTranslation", data, new DataRow("AirportTranslationId", selectedAirportTransId));
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
            nameInTextBlock.Text = LanguageComboBox.SelectedValue != null ? LanguageComboBox.SelectedValue.ToString() : string.Empty;
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

            DataRow airportTranslation = await dbCon.GetDataRow("AirportTranslation", new DataRow("AirportCode", airportCode, "LanguageCode", languageCode));
            if(airportTranslation.HasData && airportTranslation.Error == ERROR.NoError)
            {
                nameInLanguageTextBox.Text = airportTranslation.Get("AirportName").ToString();
                commitByTextBlockValue.Text = await dbCon.GetFullNameFromUid(airportTranslation.Get("CommitBy").ToString());
                commitDateTimeTextBlockValue.Text = airportTranslation.Get("CommitDateTime").ToString();

                selectedAirportTransId = airportTranslation.Get("AirportTranslationId").ToString();
            } else
            {
                DialogResult = false;
                MessageBox.Show(Messages.ERROR_GET_AIRPORT_TRANSLATION, Captions.ERROR);
                Close();
            }
        }
    }
}
