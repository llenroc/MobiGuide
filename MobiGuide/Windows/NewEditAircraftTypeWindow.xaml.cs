using System;
using System.Windows;
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
    public partial class NewEditAircraftTypeWindow : Window
    {
        private readonly DBConnector dbCon = new DBConnector();
        public NewEditAircraftTypeWindow() : this(string.Empty) { }

        public NewEditAircraftTypeWindow(string aircraftTypeCode)
        {
            InitializeComponent();

            if (aircraftTypeCode != string.Empty)
            {
                Status = STATUS.EDIT;
                AircraftTypeCode = aircraftTypeCode;
            }
            else
            {
                Status = STATUS.NEW;
            }

            if (Status == STATUS.NEW)
                Title = Messages.TITLE_NEW_AIRCRAFT_TYPE;
            else
                Title = Messages.TITLE_EDIT_AIRCRAFT_TYPE;
        }

        private STATUS Status { get; }
        private string AircraftTypeCode { get; }
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
                    break;
                case STATUS.EDIT:
                    aircraftTypeCodeTextBox.IsEnabled = false;
                    LoadTextTemplateData();
                    break;

            }
        }

        private async void LoadTextTemplateData()
        {
            try
            {
                DataRow aircraftType = await dbCon.GetDataRow("AircraftTypeReference", new DataRow("AircraftTypeCode", AircraftTypeCode));
                if(aircraftType.HasData && aircraftType.Error == ERROR.NoError)
                {
                    aircraftTypeCodeTextBox.Text = aircraftType.Get("AircraftTypeCode").ToString();
                    aircraftTypeNameTextBox.Text = aircraftType.Get("AircraftTypeName").ToString();
                    statusComboBox.SelectedValue = aircraftType.Get("StatusCode");
                    commitByTextBlockValue.Text = await dbCon.GetFullNameFromUid(aircraftType.Get("CommitBy").ToString());
                    commitTimeTextBlockValue.Text = aircraftType.Get("CommitDateTime").ToString();
                } else
                {
                    throw new Exception();
                }
            } catch (Exception)
            {
                MessageBox.Show(Messages.ERROR_GET_AIRCRAFT_TYPE, Captions.ERROR);
                DialogResult = false;
                Close();
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveAircraftType();
        }

        private async void SaveAircraftType()
        {
            saveBtn.IsEnabled = false;
            if (aircraftTypeCodeTextBox.Text.IsNull() || aircraftTypeNameTextBox.Text.IsNull())
            {
                MessageBox.Show(Messages.WARNING_NOT_FILLED_FIELDS, Captions.WARNING);
                return;
            }
            DataRow aircraftType = new DataRow(
                    "AircraftTypeCode", aircraftTypeCodeTextBox.Text,
                    "AircraftTypeName", aircraftTypeNameTextBox.Text,
                    "StatusCode", statusComboBox.SelectedValue,
                    "CommitBy", Application.Current.Resources["UserAccountId"],
                    "CommitDateTime", DateTime.Now
                );
            try
            {
                if (Status == STATUS.NEW)
                {
                    bool result = await dbCon.CreateNewRow("AircraftTypeReference", aircraftType, null);
                    if (result)
                    {
                        MessageBox.Show(Messages.SUCCESS_ADD_AIRCRAFT_TYPE, Captions.SUCCESS);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(Messages.ERROR_ADD_AIRCRAFT_TYPE, Captions.ERROR);
                        saveBtn.IsEnabled = true;
                    }
                }
                else
                {
                    bool result = await dbCon.UpdateDataRow("AircraftTypeReference", aircraftType, new DataRow("AircraftTypeCode", AircraftTypeCode));
                    if (result)
                    {
                        MessageBox.Show(Messages.SUCCESS_UPDATE_AIRCRAFT_TYPE, Captions.SUCCESS);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(Messages.ERROR_UPDATE_AIRCRAFT_TYPE, Captions.ERROR);
                        saveBtn.IsEnabled = true;
                    }
                }
            }
            catch (Exception)
            {
                if (Status == STATUS.NEW)
                {
                    MessageBox.Show(Messages.ERROR_ADD_AIRCRAFT_TYPE, Captions.ERROR);
                }
                else
                {
                    MessageBox.Show(Messages.ERROR_UPDATE_AIRCRAFT_TYPE, Captions.ERROR);
                }
                DialogResult = false;
                Close();
            }
        }
    }
}

