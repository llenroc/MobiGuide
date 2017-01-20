using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
using CustomExtensions;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for NewEditAirportReferenceWindow.xaml
    /// </summary>
    public partial class NewEditAircraftTypeWindow : Window
    {
        private STATUS Status { get; set; }
        private string AircraftTypeCode { get; set; }
        public ObservableCollection<TextTranslation> TextTranslationList { get; set; }
        DBConnector dbCon = new DBConnector();
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        public NewEditAircraftTypeWindow() : this(String.Empty) { }
        public NewEditAircraftTypeWindow(string aircraftTypeCode)
        {
            InitializeComponent();

            if (aircraftTypeCode != String.Empty)
            {
                this.Status = STATUS.EDIT;
                this.AircraftTypeCode = aircraftTypeCode;
            }
            else this.Status = STATUS.NEW;

            if (Status == STATUS.NEW)
            {
                this.Title = "New Aircraft Type";
            }
            else
            {
                this.Title = "Edit Aircraft Type";
            }
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
                MessageBox.Show("Failed to download Aircraft Type Data", "ERROR");
                DialogResult = false;
                this.Close();
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
                MessageBox.Show("Please fill Aircraft Type Code and/or Aircraft Type Name", "WARNING");
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
                        MessageBox.Show("Create Aircraft Type Successfully.", "SUCCESS");
                        DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to create Aircraft Type.", "ERROR");
                        saveBtn.IsEnabled = true;
                    }
                }
                else
                {
                    bool result = await dbCon.UpdateDataRow("AircraftTypeReference", aircraftType, new DataRow("AircraftTypeCode", AircraftTypeCode));
                    if (result)
                    {
                        MessageBox.Show("Update Aircraft Type Successfully.", "SUCCESS");
                        DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update Aircraft Type.", "ERROR");
                        saveBtn.IsEnabled = true;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to create/update Aircraft Type", "ERROR");
                DialogResult = false;
                this.Close();
            }
        }
    }
}

