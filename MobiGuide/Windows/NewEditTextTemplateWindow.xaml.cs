using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DatabaseConnector;
using System.Collections.ObjectModel;
using Properties;
using CustomExtensions;
using MobiGuide.Class;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for NewEditAirportReferenceWindow.xaml
    /// </summary>
    public partial class NewEditTextTemplateWindow : Window
    {
        private readonly DBConnector dbCon = new DBConnector();
        public NewEditTextTemplateWindow() : this(Guid.Empty) { }

        public NewEditTextTemplateWindow(Guid textTemplateId)
        {
            InitializeComponent();

            if (textTemplateId != Guid.Empty)
            {
                Status = STATUS.EDIT;
                TextTemplateId = textTemplateId;
            }
            else
            {
                Status = STATUS.NEW;
            }

            if (Status == STATUS.NEW)
                Title = Messages.TITLE_NEW_TEXT_TEMPLATE;
            else
                Title = Messages.TITLE_EDIT_TEXT_TEMPLATE;
        }

        private STATUS Status { get; }
        private Guid TextTemplateId { get; }
        public ObservableCollection<TextTranslation> TextTranslationList;

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Inactive", Value = "I" });

            // add blank row if new or load current TextTranslation if edit
            switch (Status)
            {
                case STATUS.NEW:
                    commitByStackPanel.Visibility = Visibility.Collapsed;
                    commitTimeStackPanel.Visibility = Visibility.Collapsed;
                    InitializeTextTranslationList();
                    break;
                case STATUS.EDIT:
                    LoadTextTemplateData();
                    InitializeTextTranslationList();
                    break;

            }
        }

        private async void LoadTextTemplateData()
        {
            try
            {
                DataRow textTemplate = await dbCon.GetDataRow("TextTemplate", new DataRow("TextTemplateId", TextTemplateId));
                if(textTemplate.HasData && textTemplate.Error == ERROR.NoError)
                {
                    textNameTextBox.Text = textTemplate.Get("TextName").ToString();
                    textTemplateTextBox.Text = textTemplate.Get("TextTemplate").ToString();
                    rotateInSecondsUpDown.Value = (int)textTemplate.Get("RotateInSeconds");
                    statusComboBox.SelectedValue = textTemplate.Get("StatusCode");
                    commitByTextBlockValue.Text = await dbCon.GetFullNameFromUid(textTemplate.Get("CommitBy").ToString());
                    commitTimeTextBlockValue.Text = textTemplate.Get("CommitDateTime").ToString();
                } else
                {
                    throw new Exception();
                }
            } catch (Exception)
            {
                MessageBox.Show(Messages.ERROR_GET_TEXT_TEMPLATE, Captions.ERROR);
                DialogResult = false;
                Close();
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveTextTemplateAndTranslations();
        }

        private async void SaveTextTemplateAndTranslations()
        {
            saveBtn.IsEnabled = false;
            if(textNameTextBox.Text.IsNull() || textTemplateTextBox.Text.IsNull())
            {
                MessageBox.Show(Messages.WARNING_NOT_FILLED_FIELDS, Captions.WARNING);
                return;
            }
            foreach(TextTranslation tt in TextTranslationList)
                if(!tt.LanguageCode.IsNull() && tt.TextTemplate.IsNull())
                {
                    MessageBox.Show(Messages.WARNING_NOT_FILLED_LANGUAGES, Captions.WARNING);
                    return;
                }
            DataRow textTemplate = new DataRow(
                    "AirlineCode", Application.Current.Resources["AirlineCode"],
                    "TextName", textNameTextBox.Text,
                    "TextTemplate", textTemplateTextBox.Text,
                    "RotateInSeconds", rotateInSecondsUpDown.Value,
                    "StatusCode", statusComboBox.SelectedValue,
                    "CommitBy", Application.Current.Resources["UserAccountId"],
                    "CommitDateTime", DateTime.Now
                );
            try
            {
                if (Status == STATUS.NEW)
                {
                    DataRow result = await dbCon.CreateNewRowAndGetUId("TextTemplate", textTemplate, "TextTemplateId");
                    if (result.HasData && result.Error == ERROR.NoError)
                    {
                        foreach (TextTranslation tt in TextTranslationList)
                        {
                            DataRow textTranslation = new DataRow(
                                    "TextTemplateId", result.Get("TextTemplateId"),
                                    "LanguageCode", tt.LanguageCode,
                                    "TextTemplate", tt.TextTemplate,
                                    "CommitBy", Application.Current.Resources["UserAccountId"],
                                    "CommitDateTime", DateTime.Now
                                );
                            await dbCon.CreateNewRow("TextTranslation", textTranslation, "TextTranslationId");
                        }
                        MessageBox.Show(Messages.SUCCESS_ADD_TEXT_TEMPLATE, Captions.SUCCESS);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(Messages.ERROR_ADD_TEXT_TEMPLATE, Captions.ERROR);
                        saveBtn.IsEnabled = true;
                    }
                }
                else
                {
                    bool result = await dbCon.UpdateDataRow("TextTemplate", textTemplate, new DataRow("TextTemplateId", TextTemplateId));
                    if (result)
                    {
                        foreach (TextTranslation tt in TextTranslationList)
                            if (tt.TextTranslationId != Guid.Empty)
                            {
                                DataRow textTranslation = new DataRow(
                                    "LanguageCode", tt.LanguageCode,
                                    "TextTemplate", tt.TextTemplate,
                                    "CommitBy", Application.Current.Resources["UserAccountId"],
                                    "CommitDateTime", DateTime.Now
                                );
                                await dbCon.UpdateDataRow("TextTranslation", textTranslation, new DataRow("TextTranslationId", tt.TextTranslationId, "TextTemplateId", tt.TextTemplateId));
                            }
                            else
                            {
                                DataRow textTranslation = new DataRow(
                                    "TextTemplateId", TextTemplateId,
                                    "LanguageCode", tt.LanguageCode,
                                    "TextTemplate", tt.TextTemplate,
                                    "CommitBy", Application.Current.Resources["UserAccountId"],
                                    "CommitDateTime", DateTime.Now
                                );
                                await dbCon.CreateNewRow("TextTranslation", textTranslation, "TextTranslationId");
                            }
                        MessageBox.Show(Messages.SUCCESS_UPDATE_TEXT_TEMPLATE, Captions.SUCCESS);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(Messages.ERROR_UPDATE_TEXT_TEMPLATE, Captions.ERROR);
                        saveBtn.IsEnabled = true;
                    }
                }
            } catch (Exception)
            {
                if (Status == STATUS.NEW)
                {
                    MessageBox.Show(Messages.ERROR_ADD_TEXT_TEMPLATE, Captions.ERROR);
                }
                else
                {
                    MessageBox.Show(Messages.ERROR_UPDATE_TEXT_TEMPLATE, Captions.ERROR);
                }
                DialogResult = false;
                Close();
            }
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox langComboBox = sender as ComboBox;
            if (langComboBox != null && langComboBox.SelectedIndex > -1)
                if (textTransDataGrid.SelectedItem != null)
                {
                    int indexOfItemInTextTranslationList = TextTranslationList.IndexOf(textTransDataGrid.SelectedItem as TextTranslation);
                    if (TextTranslationList[indexOfItemInTextTranslationList].IsEnabled)
                    {
                        string selectedLanguageCode = langComboBox.SelectedItem.ToString();
                        DataRow languageDetail = await dbCon.GetDataRow("LanguageReference", new DataRow("LanguageCode", selectedLanguageCode));
                        if(languageDetail.HasData && languageDetail.Error == ERROR.NoError)
                        {
                            string selectedLanguageName = languageDetail.Get("LanguageName").ToString();

                            TextDisplayWindow textDisplayWindow = new TextDisplayWindow(selectedLanguageCode, selectedLanguageName);
                            textDisplayWindow.ShowDialog();

                            if(textDisplayWindow.DialogResult.HasValue && textDisplayWindow.DialogResult.Value)
                            {
                                string textTemplate = textDisplayWindow.DisplayText;

                                TextTranslationList[indexOfItemInTextTranslationList].LanguageName = selectedLanguageName;
                                TextTranslationList[indexOfItemInTextTranslationList].IsEnabled = false;
                                TextTranslationList[indexOfItemInTextTranslationList].TextTemplate = textTemplate;
                                textTransDataGrid.CurrentCell = new DataGridCellInfo(textTransDataGrid.SelectedItem, textTransDataGrid.Columns[0]);
                                textTransDataGrid.CommitEdit();

                                TextTranslation tt = new TextTranslation();
                                List<string> excepts = new List<string>();
                                foreach(TextTranslation ttData in TextTranslationList)
                                    if (!string.IsNullOrWhiteSpace(ttData.LanguageCode)) excepts.Add(ttData.LanguageCode);
                                tt.LanguageList = await LoadLanguageList(excepts.ToArray());
                                if(tt.LanguageList.Count > 0) TextTranslationList.Add(tt);
                            } else
                            {
                                textTransDataGrid.BeginEdit();
                                textTransDataGrid.CancelEdit();
                            }
                        }
                        textTransDataGrid.SelectedIndex = -1;
                    }
                }
        }

        private async Task<List<string>> LoadLanguageList(params string[] excepts)
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
            List<string> list = new List<string>();
            if (languageList.HasData && languageList.Error == ERROR.NoError)
            {
                foreach(DataRow row in languageList)
                    list.Add(row.Get("LanguageCode").ToString());
                return list;
            }
            return list;
        }

        private void TextTranslationList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                //Row Added
            }
        }

        private async void InitializeTextTranslationList()
        {
            TextTranslationList = new ObservableCollection<TextTranslation>();
            TextTranslationList.CollectionChanged += TextTranslationList_CollectionChanged;
            textTransDataGrid.ItemsSource = TextTranslationList;

            //Add existing translation to datagrid
            //Add uncommit language translation to last row as a combobox itemssource
            TextTranslation tt;
            List<string> excepts = new List<string>();
            if(Status == STATUS.EDIT)
            {
                DataList list = await dbCon.GetDataList("TextTranslation", new DataRow("TextTemplateId", TextTemplateId));
                if(list.HasData && list.Error == ERROR.NoError)
                    foreach(DataRow row in list)
                    {
                        tt = new TextTranslation
                        {
                            TextTranslationId = (Guid)row.Get("TextTranslationId"),
                            TextTemplateId = (Guid)row.Get("TextTemplateId"),
                            LanguageCode = row.Get("LanguageCode").ToString(),
                            LanguageName = (await dbCon.GetDataRow("LanguageReference", 
                                new DataRow("LanguageCode", row.Get("LanguageCode").ToString()))).Get("LanguageName").ToString(),
                            TextTemplate = row.Get("TextTemplate").ToString(),
                            IsEnabled = false
                        };
                        TextTranslationList.Add(tt);
                        excepts.Add(row.Get("LanguageCode").ToString());
                    }
            }
            tt = new TextTranslation();
            tt.LanguageList = await LoadLanguageList(excepts.ToArray());
            if (tt.LanguageList.Count > 0) TextTranslationList.Add(tt);
        }

        private void textTransDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditDisplayText();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            EditDisplayText();
        }

        private void EditDisplayText()
        {
            int indexOfSelectedItem = textTransDataGrid.SelectedIndex;

            if (indexOfSelectedItem > -1 && !string.IsNullOrWhiteSpace((textTransDataGrid.SelectedItem as TextTranslation).LanguageCode))
            {
                TextTranslation tt = TextTranslationList[indexOfSelectedItem];
                TextDisplayWindow textDisplayWindow = new TextDisplayWindow(tt.LanguageCode, tt.LanguageName, tt.TextTemplate);
                textDisplayWindow.ShowDialog();
                if (textDisplayWindow.DialogResult.HasValue && textDisplayWindow.DialogResult.Value)
                {
                    string textTemplate = textDisplayWindow.DisplayText;
                    TextTranslationList[indexOfSelectedItem].TextTemplate = textTemplate;
                } else
                {
                    textTransDataGrid.SelectedIndex = -1;
                }
            }
        }

        private void textTransDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedIndex > -1 && !string.IsNullOrWhiteSpace((textTransDataGrid.SelectedItem as TextTranslation).LanguageCode))
                editBtn.IsEnabled = true;
            else editBtn.IsEnabled = false;
        }
    }
}

