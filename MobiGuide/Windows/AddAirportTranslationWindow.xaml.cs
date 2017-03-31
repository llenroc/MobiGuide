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
    /// Interaction logic for AddAirportTranslationWindow.xaml
    /// </summary>
    public partial class AddAirportTranslationWindow : Window
    {
        private readonly DBConnector dbCon = new DBConnector();
        private bool isStartUp = true;

        public AddAirportTranslationWindow()
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
            {
                DialogResult = true;
                MessageBox.Show(Messages.SUCCESS_ADD_AIRPORT_TRANSLATION, Captions.SUCCESS);
                Close();
            } else
            {
                MessageBox.Show(Messages.ERROR_ADD_AIRPORT_TRANSLATION, Captions.ERROR);
            }
            saveBtn.IsEnabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FetchAirportList();
        }

        private async void FetchAirportList()
        {
            string query = Properties.Settings.Default.GetUntranslateAirportListQuery.Replace(Environment.NewLine, " ").Replace("\t", " ");
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
                MessageBox.Show(Messages.ERROR_AIRPORT_REF_NOT_FOUND, Captions.WARNING);
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
            string query = string.Format("SELECT * FROM LanguageReference WHERE LanguageCode NOT IN (SELECT LanguageCode FROM AirportTranslation WHERE AirportCode = '{0}') ORDER BY LanguageCode", airportCode);
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
                    "AirportCode", airportNameComboBox.SelectedValue,
                    "LanguageCode", LanguageComboBox.SelectedValue,
                    "AirportName", nameInLanguageTextBox.Text,
                    "CommitBy", Application.Current.Resources["UserAccountId"],
                    "CommitDateTime", DateTime.Now
                );
            return await dbCon.CreateNewRow("AirportTranslation", data, "AirportTranslationId");
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
            nameInTextBlock.Text = LanguageComboBox.SelectedValue != null ? LanguageComboBox.SelectedValue.ToString() : string.Empty;
        }
    }
}
