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
using System.Collections;

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

