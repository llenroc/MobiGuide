using System;
using System.Windows;
using System.Windows.Input;
using Properties;
using DatabaseConnector;
using System.Timers;

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
        private int guidanceTime { get; set; }
        private Timer timer;
        private SeatMapPage seatMapPage;
        public DisplayWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            if (e.Key == Key.T && DisplayType != DISPLAY_TYPE.SEATMAPS)
            {
                if (timer != null && timer.Enabled) timer.Enabled = false;
                seatMapPage = new SeatMapPage(aircraftConfigurationId, frontDoorUsingFlag, rearDoorUsingFlag, "C_3");
                DisplayFrame.Navigate(seatMapPage);
                timer = new Timer(this.guidanceTime * 1000);
                timer.AutoReset = false;
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                switch (DisplayType)
                {
                    case DISPLAY_TYPE.LOGO:
                        DisplayFrame.Navigate(new LogoPage());
                        break;
                    case DISPLAY_TYPE.TEXT:
                        DisplayFrame.Navigate(new TextPage(showText));
                        break;
                }
            }));
        }

        public DisplayWindow(DISPLAY_TYPE type, params object[] param) : this()
        {
            this.DisplayType = type;
            if(type == DISPLAY_TYPE.TEXT)
            {
                if(param != null)
                    showText = param[0] as ShowText;
            }
            aircraftConfigurationId = new Guid(param[1].ToString());
            frontDoorUsingFlag = (bool)param[2];
            rearDoorUsingFlag = (bool)param[3];

            GetGuidanceTime();
        }

        private async void GetGuidanceTime()
        {
            DataRow airlineRef = await (new DBConnector()).GetDataRow("AirlineReference", new DataRow("AirlineCode", Application.Current.Resources["AirlineCode"]));
            if (airlineRef.HasData && airlineRef.Error == ERROR.NoError)
            {
                this.guidanceTime = (int)airlineRef.Get("ShowGuidanceInSeconds");
            }
            else this.guidanceTime = 10;
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
