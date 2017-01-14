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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using DatabaseConnector;
using System.Collections.ObjectModel;
using System.Reflection;
using Properties;
using System.Text.RegularExpressions;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for NewEditAirportReferenceWindow.xaml
    /// </summary>
    public partial class NewEditAirportReferenceWindow : Window
    {
        public STATUS Status { get; set; }
        public ObservableCollection<AirportTranslation> AirportTranslationList { get; set; }
        DBConnector dbCon = new DBConnector();
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        public NewEditAirportReferenceWindow(STATUS status)
        {
            InitializeComponent();

            this.Status = status;

            if (Status == STATUS.NEW)
            {
                this.Title = "New Airport Reference";
            }
            else
            {
                this.Title = "Edit Airport Reference";
            }
        }

        private async Task<DataList> LoadLanguageList(params string[] excepts)
        {
            string query = String.Empty;
            if(excepts.Length > 0)
            {
                query += "WHERE LanguageCode NOT IN (";
                for(int i = 0; i < excepts.Length; i++)
                {
                    query += String.Format("'{0}'", excepts[i]);
                    if (i < excepts.Length - 1) query += ", ";
                }
                query += ")";
            }
            query += " ORDER BY LanguageCode";
            DataList languageList = await dbCon.getDataList("LanguageReference", null, query);
            if (languageList.HasData && languageList.Error == ERROR.NoError)
            {
                return languageList;
            } else
            {
                return new DataList();
            }
        }

        private bool IsAirportCodeCorrect(string code)
        {
            if (code.Length == 3 && IsTextAllowed(code))
                return true;
            else return false;
        }
        private async Task<bool> IsExistingAirportCode(string code)
        {
            DataRow result = await dbCon.getDataRow("AirportReference", new DataRow("AirportCode", code));
            if (result.HasData && result.Error == ERROR.NoError) return true;
            else if (result.Error == ERROR.HasError) return true;
            else return false;
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

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (Status)
            {
                case STATUS.NEW:
                    saveAirportReferenceAndTranslation();
                    break;
            }
        }

        private async void saveAirportReferenceAndTranslation()
        {
            saveBtn.IsEnabled = false;
            if (!IsAirportCodeCorrect(airportCodeTextBox.Text))
            {
                MessageBox.Show("Airline Code Must Contains 3 Characters", "WARNING");
            }
            else if (String.IsNullOrWhiteSpace(airportNameTextBox.Text))
            {
                MessageBox.Show("Please Enter Airline Name", "WARNING");
            }
            else
            {
                if (await IsExistingAirportCode(airportCodeTextBox.Text.ToUpper()))
                {
                    MessageBox.Show("Airport Code is Existing", "WARNING");
                }
                else
                {
                    DataRow airportReferenceData = new DataRow(
                        "AirportCode", airportCodeTextBox.Text.ToUpper(),
                        "AirportName", airportNameTextBox.Text,
                        "StatusCode", statusComboBox.SelectedValue.ToString(),
                        "CommitBy", Application.Current.Resources["UserAccountId"],
                        "CommitDateTime", DateTime.Now
                        );
                    DataList airportTranslationDataList = new DataList();
                    foreach(AirportTranslation at in AirportTranslationList)
                    {
                        if(at.SelectedLanguageCode != null && String.IsNullOrWhiteSpace(at.AirportName))
                        {
                            MessageBox.Show("Please fill all display name in different languages", "WARNING");
                            saveBtn.IsEnabled = true;
                            return;
                        }
                        if(at.SelectedLanguageCode != null && !String.IsNullOrWhiteSpace(at.AirportName))
                        {
                            DataRow airportTranslationData = new DataRow(
                                "AirportCode", airportCodeTextBox.Text.ToUpper(),
                                "LanguageCode", at.SelectedLanguageCode,
                                "AirportName", at.AirportName,
                                "CommitBy", Application.Current.Resources["UserAccountId"],
                                "CommitDateTime", DateTime.Now
                            );
                            airportTranslationDataList.Add(airportTranslationData);
                        }
                    }
                    if (await dbCon.createNewRow("AirportReference", airportReferenceData, null))
                    {
                        foreach(DataRow row in airportTranslationDataList)
                        {
                            await dbCon.createNewRow("AirportTranslation", row, "AirportTranslationId");
                        }
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

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox langComboBox = sender as ComboBox;
            if(langComboBox != null)
            {
                if(airportTransDataGrid.SelectedItem != null)
                {
                    if (AirportTranslationList[airportTransDataGrid.SelectedIndex].IsEnabled == true)
                    {
                        DataRow languageDetail = await dbCon.getDataRow("LanguageReference",
                        new DataRow("LanguageCode", langComboBox.SelectedItem.ToString()));
                        if (languageDetail.HasData && languageDetail.Error == ERROR.NoError)
                        {
                            AirportTranslationList[airportTransDataGrid.SelectedIndex].LanguageName = languageDetail.Get("LanguageName").ToString();
                            AirportTranslationList[airportTransDataGrid.SelectedIndex].IsEnabled = false;

                            airportTransDataGrid.CurrentCell = new DataGridCellInfo(airportTransDataGrid.SelectedItem, airportTransDataGrid.Columns[2]);
                            airportTransDataGrid.BeginEdit();

                            List<string> exceptCodes = new List<string>();
                            exceptCodes.Add((sender as ComboBox).SelectedItem.ToString());
                            for (int i = 0; i < AirportTranslationList.Count; i++)
                            {
                                if (AirportTranslationList[i].SelectedLanguageCode != null && !String.IsNullOrWhiteSpace(AirportTranslationList[i].SelectedLanguageCode.ToString()))
                                {

                                    exceptCodes.Add(AirportTranslationList[i].SelectedLanguageCode.ToString());
                                }
                            }
                            DataList availableLanguage = await LoadLanguageList(exceptCodes.ToArray());
                            if(availableLanguage.Count > 0)
                            {
                                AirportTranslation at = new AirportTranslation();
                                foreach (DataRow row in availableLanguage)
                                {
                                    at.LanguageList.Add(row.Get("LanguageCode").ToString());
                                }
                                AirportTranslationList.Add(at);
                            }
                        }
                    }
                }
            }
        }
        void AirportTranslationList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                //Row Added
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            statusComboBox.Items.Add(new ComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new ComboBoxItem { Text = "Inactive", Value = "I" });

            // add blank row if new or load current airportTranslation if edit
            switch (Status)
            {
                case STATUS.NEW:
                    commitByStackPanel.Visibility = Visibility.Collapsed;
                    commitTimeStackPanel.Visibility = Visibility.Collapsed;

                    AirportTranslationList = new ObservableCollection<AirportTranslation>();
                    AirportTranslation at = new AirportTranslation();
                    foreach (DataRow row in await LoadLanguageList())
                    {
                        at.LanguageList.Add(row.Get("LanguageCode").ToString());
                    }
                    AirportTranslationList.Add(at);
                    AirportTranslationList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(AirportTranslationList_CollectionChanged);
                    airportTransDataGrid.ItemsSource = AirportTranslationList;
                    break;
            }
        }
    }
}

