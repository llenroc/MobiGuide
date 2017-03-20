using System;
using System.Windows;
using DatabaseConnector;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for NewEditAirportReferenceWindow.xaml
    /// </summary>
    public partial class NewEditAircraftConfigurationWindow : Window
    {
        private STATUS Status { get; set; }
        private Guid AircraftConfigurationId { get; set; }
        private AircraftConfiguration AircraftConfiguration { get; set; }
        DBConnector dbCon = new DBConnector();
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        public NewEditAircraftConfigurationWindow() : this(Guid.Empty) { }
        public NewEditAircraftConfigurationWindow(Guid aircraftConfigId)
        {
            InitializeComponent();

            if (aircraftConfigId != Guid.Empty)
            {
                Status = STATUS.EDIT;
                this.Title = "Edit Aircraft Configuration";
            }
            else
            {
                Status = STATUS.NEW;
                this.Title = "New Aircraft Configuration";
            }
            AircraftConfigurationId = aircraftConfigId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = (SystemParameters.PrimaryScreenWidth / 2) - (this.ActualWidth / 2);
            mainFrame.Navigate(new NewEditAircraftConfigurationPage1(AircraftConfigurationId));
        }
    }
}

