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
using System.Windows.Shapes;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for DisplayWindow.xaml
    /// </summary>
    public partial class DisplayWindow : Window
    {
        public DISPLAY_TYPE DisplayType { get; set; }
        private ShowText showText { get; set; }
        private Guid aircraftConfigurationId { get; set; }
        private bool frontDoorUsingFlag { get; set; }
        private bool rearDoorUsingFlag { get; set; }
        public DisplayWindow()
        {
            InitializeComponent();
        }
        public DisplayWindow(DISPLAY_TYPE type, params object[] param) : this()
        {
            this.DisplayType = type;
            if(type == DISPLAY_TYPE.TEXT)
            {
                if(param != null)
                    showText = param[0] as ShowText;
            }
            if(type == DISPLAY_TYPE.SEATMAPS)
            {
                aircraftConfigurationId = new Guid(param[0].ToString());
                frontDoorUsingFlag = (bool)param[1];
                rearDoorUsingFlag = (bool)param[2];
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (DisplayType)
            {
                case DISPLAY_TYPE.LOGO:
                    DisplayFrame.Navigate(new LogoPage());
                    break;
                case DISPLAY_TYPE.TEXT:
                    DisplayFrame.Navigate(new TextPage(showText));
                    break;
                case DISPLAY_TYPE.SEATMAPS:
                    DisplayFrame.Navigate(new SeatMapPage(aircraftConfigurationId, frontDoorUsingFlag, rearDoorUsingFlag));
                    break;
            }
        }
    }
}
