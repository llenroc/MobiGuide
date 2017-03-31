using System;
using System.Windows;
using DatabaseConnector;
using MobiGuide.Class;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for NewEditAirportReferenceWindow.xaml
    /// </summary>
    public partial class NewEditAircraftConfigurationWindow : Window
    {
        private DBConnector dbCon = new DBConnector();
        public NewEditAircraftConfigurationWindow() : this(Guid.Empty) { }

        public NewEditAircraftConfigurationWindow(Guid aircraftConfigId)
        {
            InitializeComponent();

            if (aircraftConfigId != Guid.Empty)
            {
                Status = STATUS.EDIT;
                Title = Messages.TITLE_EDIT_AIRCRAFT_CONFIG;
            }
            else
            {
                Status = STATUS.NEW;
                Title = Messages.TITLE_NEW_AIRCRAFT_CONFIG;
            }
            AircraftConfigurationId = aircraftConfigId;
        }

        private STATUS Status;
        private Guid AircraftConfigurationId { get; }
        private AircraftConfiguration AircraftConfiguration;

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Left = SystemParameters.PrimaryScreenWidth / 2 - ActualWidth / 2;
            mainFrame.Navigate(new NewEditAircraftConfigurationPage1(AircraftConfigurationId));
        }
    }
}

