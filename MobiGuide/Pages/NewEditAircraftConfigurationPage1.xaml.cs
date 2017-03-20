using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Properties;
using DatabaseConnector;
using System.Reflection;
using System.Diagnostics;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for NewEditAircraftConfigurationPage1.xaml
    /// </summary>
    public partial class NewEditAircraftConfigurationPage1 : Page
    {
        DBConnector dbCon = new DBConnector();
        public STATUS Status { get; set; }
        private Guid AircraftConfigId { get; set; }
        private Window window
        {
            get
            {
                DependencyObject parent = VisualTreeHelper.GetParent(this);
                while (!(parent is Window))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
                return parent as Window;
            }
        }
        private AircraftConfiguration AircraftConfiguration { get; set; }
        public NewEditAircraftConfigurationPage1() : this(Guid.Empty) { }
        public NewEditAircraftConfigurationPage1(Guid aircraftConfigId)
        {
            InitializeComponent();
            if (aircraftConfigId == Guid.Empty)
            {
                this.Status = STATUS.NEW;
            }
            else
            {
                AircraftConfigId = aircraftConfigId;
                this.Status = STATUS.EDIT;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NewEditAircraftConfigurationWindow window = this.window as NewEditAircraftConfigurationWindow;
            window.Top = (SystemParameters.PrimaryScreenHeight / 2) - (window.ActualHeight / 2);
            DataList aircraftTypeDataList = await dbCon.GetDataList("AircraftTypeReference", null, "WHERE StatusCode = 'A' ORDER BY AircraftTypeCode");
            if(aircraftTypeDataList.HasData && aircraftTypeDataList.Error == ERROR.NoError)
            {
                foreach(DataRow row in aircraftTypeDataList)
                {
                    aircraftTypeComboBox.Items.Add(new CustomComboBoxItem {
                        Text = String.Format("{0} - {1}", row.Get("AircraftTypeCode").ToString(), 
                                        row.Get("AircraftTypeName").ToString()),
                        Value = row.Get("AircraftTypeCode").ToString()
                    });
                }
            } else
            {
                MessageBoxResult result = MessageBox.Show("No Aircraft Type Found. Do you want to go to create it?", "No Aircraft Type Found", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    this.window.Hide();
                    AircraftTypeWindow aircraftTypeWindow = new AircraftTypeWindow();
                    aircraftTypeWindow.ShowDialog();
                    this.window.Close();
                }
                else
                {
                    this.window.Close();
                }
            }

            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Inactive", Value = "I" });

            if (Status == STATUS.NEW)
            {
                commitByStackPanel.Visibility = Visibility.Collapsed;
                commitTimeStackPanel.Visibility = Visibility.Collapsed;
            } else
            {
                try
                {
                    DataRow acData = await dbCon.GetDataRow("AircraftConfiguration", new DataRow("AircraftConfigurationId", AircraftConfigId));
                    if (acData.HasData && acData.Error == ERROR.NoError)
                    {
                        this.AircraftConfiguration = new AircraftConfiguration();
                        PropertyInfo[] properties = this.AircraftConfiguration.GetType().GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            if (acData.ContainKey(property.Name))
                            {
                                if(property.CanWrite)
                                    property.SetValue(this.AircraftConfiguration, acData.Get(property.Name), null);
                            }
                        }
                        DataRow aircraftTypeData = await dbCon.GetDataRow("AircraftTypeReference", new DataRow("AircraftTypeCode", acData.Get("AircraftTypeCode")));
                        if (aircraftTypeData.HasData && aircraftTypeData.Error == ERROR.NoError)
                        {
                            AircraftType aircraftType = new AircraftType();
                            PropertyInfo[] atProps = aircraftType.GetType().GetProperties();
                            foreach (PropertyInfo atProp in atProps)
                            {
                                if(atProp.CanWrite)
                                    atProp.SetValue(aircraftType, aircraftTypeData.Get(atProp.Name), null);
                            }
                            this.AircraftConfiguration.AircraftType = aircraftType;

                            aircraftConfigCodeTextBox.Text = this.AircraftConfiguration.AircraftConfigurationCode;
                            aircraftConfigNameTextBox.Text = this.AircraftConfiguration.AircraftConfigurationName;
                            aircraftTypeComboBox.SelectedValue = this.AircraftConfiguration.AircraftType.AircraftTypeCode;
                            statusComboBox.SelectedValue = this.AircraftConfiguration.StatusCode;
                            commitByTextBlockValue.Text = await dbCon.GetFullNameFromUid(acData.Get("CommitBy").ToString());
                            commitTimeTextBlockValue.Text = acData.Get("CommitDateTime").ToString();
                        }
                        else
                        {
                            MessageBox.Show("Cannot get Aircraft Configuration Data", "ERROR");
                            window.DialogResult = false;
                            window.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot get Aircraft Configuration Data", "ERROR");
                        window.DialogResult = false;
                        window.Close();
                    }
                } catch (Exception ex) {
                    Debug.WriteLine(ex.Message);
                    MessageBox.Show("Cannot get Aircraft Configuration Data", "ERROR");
                    window.DialogResult = false;
                    window.Close();
                }
            }
            aircraftConfigCodeTextBox.Focus();
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(aircraftConfigCodeTextBox.Text))
            {
                MessageBox.Show("Please enter \"Aircraft Configuration Code\"", "WARNING");
                return;
            }
            if (String.IsNullOrEmpty(aircraftConfigNameTextBox.Text))
            {
                MessageBox.Show("Please enter \"Aircraft Configuration Name\"", "WARNING");
                return;
            }
            if (Status == STATUS.NEW)
                this.AircraftConfiguration = new AircraftConfiguration();
            this.AircraftConfiguration.AircraftConfigurationCode = aircraftConfigCodeTextBox.Text;
            this.AircraftConfiguration.AircraftConfigurationName = aircraftConfigNameTextBox.Text;
            this.AircraftConfiguration.AirlineCode = Application.Current.Resources["AirlineCode"].ToString();
            this.AircraftConfiguration.AircraftType = new AircraftType
            {
                AircraftTypeCode = aircraftTypeComboBox.SelectedValue.ToString()
            };
            this.AircraftConfiguration.StatusCode = statusComboBox.SelectedValue.ToString();
            (this.window as NewEditAircraftConfigurationWindow).mainFrame.Navigate(new NewEditAircraftConfigurationPage2(this.AircraftConfiguration, this.Status));
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame mainFrame = (this.window as NewEditAircraftConfigurationWindow).mainFrame;
            if (mainFrame.CanGoBack)
            {
                mainFrame.GoBack(); 
            }
        }
    }
}
