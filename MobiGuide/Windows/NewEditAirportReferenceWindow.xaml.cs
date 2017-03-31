using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DatabaseConnector;
using System.Collections.ObjectModel;
using Properties;
using System.Text.RegularExpressions;
using MobiGuide.Class;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for NewEditAirportReferenceWindow.xaml
    /// </summary>
    public partial class NewEditAirportReferenceWindow : Window
    {
        private readonly DBConnector dbCon = new DBConnector();
        public NewEditAirportReferenceWindow() : this(null) { }

        public NewEditAirportReferenceWindow(string airportCode)
        {
            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(airportCode))
            {
                Status = STATUS.EDIT;
                this.airportCode = airportCode;
            }
            else
            {
                Status = STATUS.NEW;
            }

            if (Status == STATUS.NEW)
                Title = Messages.TITLE_NEW_AIRPORT_REF;
            else
                Title = Messages.TITLE_EDIT_AIRPORT_REF;
        }

        private STATUS Status { get; }
        private string airportCode { get; }
        public ObservableCollection<AirportTranslation> AirportTranslationList;

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Inactive", Value = "I" });

            // add blank row if new or load current airportTranslation if edit
            switch (Status)
            {
                case STATUS.NEW:
                    commitByStackPanel.Visibility = Visibility.Collapsed;
                    commitTimeStackPanel.Visibility = Visibility.Collapsed;
                    InitializeAirportTranslationList(null);
                    break;
                case STATUS.EDIT:
                    DataRow airportReferenceData = await LoadAirportReference(airportCode);

                    airportCodeTextBox.Text = airportReferenceData.Get("AirportCode").ToString();
                    airportCodeTextBox.IsEnabled = false;

                    airportNameTextBox.Text = airportReferenceData.Get("AirportName").ToString();
                    statusComboBox.SelectedValue = airportReferenceData.Get("StatusCode");

                    commitByTextBlockValue.Text = await dbCon.GetFullNameFromUid(airportReferenceData.Get("CommitBy").ToString());
                    commitTimeTextBlockValue.Text = airportReferenceData.Get("CommitDateTime").ToString();

                    InitializeAirportTranslationList(airportCode);

                    break;

            }
        }

        private void airportCodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (Status)
            {
                case STATUS.NEW:
                    SaveAirportReferenceAndTranslation();
                    break;
                case STATUS.EDIT:
                    UpdateAirportReferenceAndTranslation();
                    break;
            }
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox langComboBox = sender as ComboBox;
            if(langComboBox != null)
                if(airportTransDataGrid.SelectedItem != null)
                {
                    int indexOfItemInAirportTranslationList = AirportTranslationList.IndexOf(airportTransDataGrid.SelectedItem as AirportTranslation);
                    if (AirportTranslationList[indexOfItemInAirportTranslationList].IsEnabled)
                    {
                        DataRow languageDetail = await dbCon.GetDataRow("LanguageReference",
                            new DataRow("LanguageCode", langComboBox.SelectedItem.ToString()));
                        if (languageDetail.HasData && languageDetail.Error == ERROR.NoError)
                        {
                            AirportTranslationList[indexOfItemInAirportTranslationList].LanguageName = languageDetail.Get("LanguageName").ToString();
                            AirportTranslationList[indexOfItemInAirportTranslationList].IsEnabled = false;

                            airportTransDataGrid.CurrentCell = new DataGridCellInfo(airportTransDataGrid.SelectedItem, airportTransDataGrid.Columns[2]);
                            airportTransDataGrid.BeginEdit();

                            List<string> exceptCodes = new List<string>();
                            exceptCodes.Add((sender as ComboBox).SelectedItem.ToString());
                            for (int i = 0; i < AirportTranslationList.Count; i++)
                                if (AirportTranslationList[i].LanguageCode != null && !string.IsNullOrWhiteSpace(AirportTranslationList[i].LanguageCode))
                                    exceptCodes.Add(AirportTranslationList[i].LanguageCode);
                            DataList availableLanguage = await LoadLanguageList(exceptCodes.ToArray());
                            if(availableLanguage.Count > 0)
                            {
                                AirportTranslation at = new AirportTranslation();
                                foreach (DataRow row in availableLanguage)
                                    at.LanguageList.Add(row.Get("LanguageCode").ToString());
                                AirportTranslationList.Add(at);
                            }
                        }
                    }
                }
        }

        private void AirportTranslationList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                //Row Added
            }
        }

        private async void InitializeAirportTranslationList(string airportCode)
        {
            AirportTranslationList = new ObservableCollection<AirportTranslation>();
            AirportTranslationList.CollectionChanged += AirportTranslationList_CollectionChanged;
            airportTransDataGrid.ItemsSource = AirportTranslationList;

            DataList atList = string.IsNullOrWhiteSpace(airportCode) ? new DataList() : await dbCon.GetDataList("AirportTranslation", new DataRow("AirportCode", airportCode), "ORDER BY LanguageCode");
            if(atList.HasData && atList.Error == ERROR.NoError)
            {
                AirportTranslation at = null;
                List<string> exceptedLanguageCodes = new List<string>();
                foreach(DataRow row in atList)
                {
                    at = new AirportTranslation();
                    at.AirportTranslationId = (Guid)row.Get("AirportTranslationId");
                    at.AirportName = row.Get("AirportName").ToString();
                    at.LanguageCode = row.Get("LanguageCode").ToString();
                    at.LanguageName = (await dbCon.GetDataRow("LanguageReference", 
                        new DataRow("LanguageCode", row.Get("LanguageCode")))).Get("LanguageName").ToString();
                    at.IsEnabled = false;
                    AirportTranslationList.Add(at);
                    exceptedLanguageCodes.Add(row.Get("LanguageCode").ToString());
                }
                at = new AirportTranslation();
                foreach (DataRow row in await LoadLanguageList(exceptedLanguageCodes.ToArray()))
                    at.LanguageList.Add(row.Get("LanguageCode").ToString());
                if(at.LanguageList.Count > 0) AirportTranslationList.Add(at);
            }
            else
            {
                AirportTranslation at = new AirportTranslation();
                DataList LanguageList = await LoadLanguageList();
                if(LanguageList.Count == 0)
                {
                    MessageBox.Show(Messages.ERROR_LANGUAGE_REF_NOT_FOUND, Captions.ERROR);
                    Close();
                }
                foreach (DataRow row in LanguageList)
                    at.LanguageList.Add(row.Get("LanguageCode").ToString());
                if (at.LanguageList.Count > 0) AirportTranslationList.Add(at);
            }
        }

        private async Task<DataRow> LoadAirportReference(string airportCode)
        {
            DataRow row = await dbCon.GetDataRow("AirportReference", new DataRow("AirportCode", airportCode));
            if (row.HasData && row.Error == ERROR.NoError)
                return row;
            return new DataRow();
        }

        private async Task<DataList> LoadLanguageList(params string[] excepts)
        {
            string query = string.Empty;
            if (excepts.Length > 0)
            {
                query += "WHERE LanguageCode NOT IN (";
                for (int i = 0; i < excepts.Length; i++)
                {
                    query += string.Format("'{0}'", excepts[i]);
                    if (i < excepts.Length - 1) query += ", ";
                }
                query += ")";
            }
            query += " ORDER BY LanguageCode";
            DataList languageList = await dbCon.GetDataList("LanguageReference", null, query);
            if (languageList.HasData && languageList.Error == ERROR.NoError)
                return languageList;
            return new DataList();
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

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[a-zA-Z]"); //regex that matches disallowed text
            return regex.IsMatch(text);
        }

        private async void SaveAirportReferenceAndTranslation()
        {
            saveBtn.IsEnabled = false;
            if (!IsAirportCodeCorrect(airportCodeTextBox.Text))
            {
                MessageBox.Show(Messages.WARNING_WRONG_AIRPORT_CODE, Captions.WARNING);
            }
            else if (string.IsNullOrWhiteSpace(airportNameTextBox.Text))
            {
                MessageBox.Show(Messages.WARNING_ENTER_AIRPORT_NAME, "WARNING");
            }
            else
            {
                if (await IsExistingAirportCode(airportCodeTextBox.Text.ToUpper()))
                {
                    MessageBox.Show(Messages.WARNING_EXISTING_AIRPORT_CODE, Captions.WARNING);
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
                    foreach (AirportTranslation at in AirportTranslationList)
                    {
                        if (at.LanguageCode != null && string.IsNullOrWhiteSpace(at.AirportName))
                        {
                            MessageBox.Show(Messages.WARNING_NOT_FILLED_LANGUAGES, Captions.WARNING);
                            saveBtn.IsEnabled = true;
                            return;
                        }
                        if (at.LanguageCode != null && !string.IsNullOrWhiteSpace(at.AirportName))
                        {
                            DataRow airportTranslationData = new DataRow(
                                "AirportCode", airportCodeTextBox.Text.ToUpper(),
                                "LanguageCode", at.LanguageCode,
                                "AirportName", at.AirportName,
                                "CommitBy", Application.Current.Resources["UserAccountId"],
                                "CommitDateTime", DateTime.Now
                            );
                            airportTranslationDataList.Add(airportTranslationData);
                        }
                    }
                    if (await dbCon.CreateNewRow("AirportReference", airportReferenceData, null))
                    {
                        foreach (DataRow row in airportTranslationDataList)
                            await dbCon.CreateNewRow("AirportTranslation", row, "AirportTranslationId");
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

        private async void UpdateAirportReferenceAndTranslation()
        {
            saveBtn.IsEnabled = false;
            if (string.IsNullOrWhiteSpace(airportNameTextBox.Text))
            {
                MessageBox.Show(Messages.WARNING_ENTER_AIRPORT_NAME, Captions.WARNING);
            }
            else
            {
                DataRow airportReferenceData = new DataRow(
                    "AirportName", airportNameTextBox.Text,
                    "StatusCode", statusComboBox.SelectedValue.ToString(),
                    "CommitBy", Application.Current.Resources["UserAccountId"],
                    "CommitDateTime", DateTime.Now
                    );
                DataList airportTranslationDataListToCreate = new DataList();
                DataList airportTranslationDataListToUpdate = new DataList();
                foreach (AirportTranslation at in AirportTranslationList)
                {
                    if (at.LanguageCode != null && string.IsNullOrWhiteSpace(at.AirportName))
                    {
                        MessageBox.Show(Messages.WARNING_NOT_FILLED_LANGUAGES, Captions.WARNING);
                        saveBtn.IsEnabled = true;
                        return;
                    }
                    if (at.LanguageCode != null && !string.IsNullOrWhiteSpace(at.AirportName))
                        if(at.LanguageList.Count > 0)
                        {
                            DataRow airportTranslationData = new DataRow(
                                "AirportCode", airportCodeTextBox.Text.ToUpper(),
                                "LanguageCode", at.LanguageCode,
                                "AirportName", at.AirportName,
                                "CommitBy", Application.Current.Resources["UserAccountId"],
                                "CommitDateTime", DateTime.Now
                            );
                            airportTranslationDataListToCreate.Add(airportTranslationData);
                        } else
                        {
                            DataRow airportTranslationData = new DataRow(
                                "AirportTranslationId", at.AirportTranslationId,
                                "AirportName", at.AirportName,
                                "CommitBy", Application.Current.Resources["UserAccountId"],
                                "CommitDateTime", DateTime.Now
                            );
                            airportTranslationDataListToUpdate.Add(airportTranslationData);
                        }
                }
                if (await dbCon.UpdateDataRow("AirportReference", airportReferenceData, new DataRow("AirportCode", airportCodeTextBox.Text.ToUpper())))
                {
                    foreach (DataRow row in airportTranslationDataListToCreate)
                        await dbCon.CreateNewRow("AirportTranslation", row, "AirportTranslationId");
                    foreach(DataRow row in airportTranslationDataListToUpdate)
                    {
                        string airportTranslationId = row.Get("AirportTranslationId").ToString();
                        await dbCon.UpdateDataRow("AirportTranslation", row.RemoveAt(0), new DataRow("AirportTranslationId", airportTranslationId));
                    }
                    DialogResult = true;
                    MessageBox.Show(Messages.SUCCESS_ADD_AIRPORT_REF, Captions.SUCCESS);
                    Close();
                }
                else
                {
                    MessageBox.Show(Messages.ERROR_ADD_AIRPORT_REF, Captions.ERROR);
                }
            }
            saveBtn.IsEnabled = true;
        }
    }
}

