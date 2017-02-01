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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Properties;
using DatabaseConnector;
using System.Diagnostics;
using Microsoft.Win32;
using CustomExtensions;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for NewEditAircraftConfigurationPage2.xaml
    /// </summary>
    public partial class NewEditAircraftConfigurationPage2 : Page
    {
        DBConnector dbCon = new DBConnector();
        public STATUS Status { get; set; }
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
        private string seatMapImagePath { get; set; }
        public NewEditAircraftConfigurationPage2() : this(null, STATUS.NEW) { }
        public NewEditAircraftConfigurationPage2(AircraftConfiguration aircraftConfiguration, STATUS Status)
        {
            InitializeComponent();
            this.Status = Status;
            if(aircraftConfiguration != null)
            {
                this.AircraftConfiguration = aircraftConfiguration;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NewEditAircraftConfigurationWindow window = this.window as NewEditAircraftConfigurationWindow;
            window.Top = (SystemParameters.PrimaryScreenHeight / 2) - (window.ActualHeight / 2);
            if (window.mainFrame.CanGoBack)
            {
                backBtn.Visibility = Visibility.Visible;
                backBtn.IsEnabled = true;
            }
            if(Status == STATUS.EDIT && this.AircraftConfiguration.SeatMapImagePath == null)
            {
                DataRow seatMapImageData = await dbCon.GetDataRow("AircraftConfiguration", new DataRow("AircraftConfigurationId", this.AircraftConfiguration.AircraftConfigurationId));
                if(seatMapImageData.HasData && seatMapImageData.Error == ERROR.NoError)
                {
                    if(seatMapImageData.Get("SeatMapImage") != DBNull.Value)
                    {
                        seatMapImage.Source = seatMapImageData.Get("SeatMapImage").BlobToSource();
                        nextBtn.IsEnabled = true;
                    }
                }
            }
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            this.AircraftConfiguration.SeatMapImagePath = this.seatMapImagePath;
            (this.window as NewEditAircraftConfigurationWindow).mainFrame.Navigate(new NewEditAircraftConfigurationPage3(this.AircraftConfiguration, Status));
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame mainFrame = (this.window as NewEditAircraftConfigurationWindow).mainFrame;
            if (mainFrame.CanGoBack)
            {
                mainFrame.GoBack(); 
            }
        }

        private void browseBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog imgofDialog = new OpenFileDialog();
            imgofDialog.Filter = "Image Files|*.jpg;*.bmp;*.png";
            imgofDialog.FilterIndex = 1;

            if (imgofDialog.ShowDialog() == true)
            {
                string filePath = imgofDialog.FileName;
                sourcePathTextBox.Text = filePath;

                seatMapImage.Source = new BitmapImage(new Uri(filePath));
                seatMapImagePath = filePath;

                if (seatMapImagePath != null) nextBtn.IsEnabled = true;
            }
        }
    }
}
