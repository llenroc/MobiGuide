using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DatabaseConnector;
using MobiGuide.Class;
using Properties;

namespace MobiGuide
{
    /// <summary>
    ///     Interaction logic for NewEditAircraftConfigurationPage1.xaml
    /// </summary>
    public partial class NewEditAircraftConfigurationPage1 : Page
    {
        private readonly DBConnector dbCon = new DBConnector();

        public NewEditAircraftConfigurationPage1() : this(Guid.Empty)
        {
        }

        public NewEditAircraftConfigurationPage1(Guid aircraftConfigId)
        {
            InitializeComponent();
            if (aircraftConfigId == Guid.Empty)
            {
                Status = STATUS.NEW;
            }
            else
            {
                AircraftConfigId = aircraftConfigId;
                Status = STATUS.EDIT;
            }
        }

        public STATUS Status;
        private Guid AircraftConfigId { get; }

        private Window window
        {
            get
            {
                var parent = VisualTreeHelper.GetParent(this);
                while (!(parent is Window))
                    parent = VisualTreeHelper.GetParent(parent);
                return parent as Window;
            }
        }

        private AircraftConfiguration AircraftConfiguration;

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.window as NewEditAircraftConfigurationWindow;
            window.Top = SystemParameters.PrimaryScreenHeight / 2 - window.ActualHeight / 2;
            var aircraftTypeDataList = await dbCon.GetDataList("AircraftTypeReference", null,
                "WHERE StatusCode = 'A' ORDER BY AircraftTypeCode");
            if (aircraftTypeDataList.HasData && aircraftTypeDataList.Error == ERROR.NoError)
            {
                foreach (DataRow row in aircraftTypeDataList)
                    aircraftTypeComboBox.Items.Add(new CustomComboBoxItem
                    {
                        Text = string.Format("{0} - {1}", row.Get("AircraftTypeCode"),
                            row.Get("AircraftTypeName")),
                        Value = row.Get("AircraftTypeCode").ToString()
                    });
            }
            else
            {
                var result = MessageBox.Show(Messages.ERROR_AIRCRAFT_TYPE_NOTFOUND,
                    Captions.NO_AIRCRAFT_TYPE_FOUND, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    this.window.Hide();
                    var aircraftTypeWindow = new AircraftTypeWindow();
                    aircraftTypeWindow.ShowDialog();
                    this.window.Close();
                }
                else
                {
                    this.window.Close();
                }
            }

            statusComboBox.Items.Add(new CustomComboBoxItem {Text = "Active", Value = "A"});
            statusComboBox.Items.Add(new CustomComboBoxItem {Text = "Inactive", Value = "I"});

            if (Status == STATUS.NEW)
            {
                commitByStackPanel.Visibility = Visibility.Collapsed;
                commitTimeStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                try
                {
                    var acData = await dbCon.GetDataRow("AircraftConfiguration",
                        new DataRow("AircraftConfigurationId", AircraftConfigId));
                    if (acData.HasData && acData.Error == ERROR.NoError)
                    {
                        AircraftConfiguration = new AircraftConfiguration();
                        var properties = AircraftConfiguration.GetType().GetProperties();
                        foreach (var property in properties)
                            if (acData.ContainKey(property.Name))
                                if (property.CanWrite)
                                    property.SetValue(AircraftConfiguration, acData.Get(property.Name), null);
                        var aircraftTypeData = await dbCon.GetDataRow("AircraftTypeReference",
                            new DataRow("AircraftTypeCode", acData.Get("AircraftTypeCode")));
                        if (aircraftTypeData.HasData && aircraftTypeData.Error == ERROR.NoError)
                        {
                            var aircraftType = new AircraftType();
                            var atProps = aircraftType.GetType().GetProperties();
                            foreach (var atProp in atProps)
                                if (atProp.CanWrite)
                                    atProp.SetValue(aircraftType, aircraftTypeData.Get(atProp.Name), null);
                            AircraftConfiguration.AircraftType = aircraftType;

                            aircraftConfigCodeTextBox.Text = AircraftConfiguration.AircraftConfigurationCode;
                            aircraftConfigNameTextBox.Text = AircraftConfiguration.AircraftConfigurationName;
                            aircraftTypeComboBox.SelectedValue = AircraftConfiguration.AircraftType.AircraftTypeCode;
                            statusComboBox.SelectedValue = AircraftConfiguration.StatusCode;
                            commitByTextBlockValue.Text =
                                await dbCon.GetFullNameFromUid(acData.Get("CommitBy").ToString());
                            commitTimeTextBlockValue.Text = acData.Get("CommitDateTime").ToString();
                        }
                        else
                        {
                            MessageBox.Show(Messages.ERROR_GET_AIRCRAFT_CONFIG, Captions.ERROR);
                            window.DialogResult = false;
                            window.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show(Messages.ERROR_GET_AIRCRAFT_CONFIG, Captions.ERROR);
                        window.DialogResult = false;
                        window.Close();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    MessageBox.Show(Messages.ERROR_GET_AIRCRAFT_CONFIG, Captions.ERROR);
                    window.DialogResult = false;
                    window.Close();
                }
            }
            aircraftConfigCodeTextBox.Focus();
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(aircraftConfigCodeTextBox.Text))
            {
                MessageBox.Show(Messages.WARNING_ENTER_AIRCRAFT_CONFIG_CODE, Captions.WARNING);
                return;
            }
            if (string.IsNullOrEmpty(aircraftConfigNameTextBox.Text))
            {
                MessageBox.Show(Messages.WARNING_ENTER_AIRCRAFT_CONFIG_NAME, Captions.WARNING);
                return;
            }
            if (Status == STATUS.NEW)
                AircraftConfiguration = new AircraftConfiguration();
            AircraftConfiguration.AircraftConfigurationCode = aircraftConfigCodeTextBox.Text;
            AircraftConfiguration.AircraftConfigurationName = aircraftConfigNameTextBox.Text;
            AircraftConfiguration.AirlineCode = Application.Current.Resources["AirlineCode"].ToString();
            AircraftConfiguration.AircraftType = new AircraftType
            {
                AircraftTypeCode = aircraftTypeComboBox.SelectedValue.ToString()
            };
            AircraftConfiguration.StatusCode = statusComboBox.SelectedValue.ToString();
            (window as NewEditAircraftConfigurationWindow).mainFrame.Navigate(
                new NewEditAircraftConfigurationPage2(AircraftConfiguration, Status));
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainFrame = (window as NewEditAircraftConfigurationWindow).mainFrame;
            if (mainFrame.CanGoBack)
                mainFrame.GoBack();
        }
    }
}