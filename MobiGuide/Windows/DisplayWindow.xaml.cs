using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using DatabaseConnector;
using Properties;

namespace MobiGuide
{
    /// <summary>
    ///     Interaction logic for DisplayWindow.xaml
    /// </summary>
    public partial class DisplayWindow : Window
    {
        private SeatMapPage seatMapPage;
        private Timer timer;

        public DisplayWindow()
        {
            InitializeComponent();

            PreviewKeyDown += HandleEsc;
        }

        public DisplayWindow(DISPLAY_TYPE type, params object[] param) : this()
        {
            DisplayType = type;
            if (type == DISPLAY_TYPE.TEXT)
                if (param != null)
                    showText = param[0] as ShowText;
            aircraftConfigurationId = new Guid(param[1].ToString());
            frontDoorUsingFlag = (bool) param[2];
            rearDoorUsingFlag = (bool) param[3];

            GetGuidanceTime();
        }

        public DISPLAY_TYPE DisplayType;
        private ShowText showText { get; }
        private Guid aircraftConfigurationId { get; }
        private bool frontDoorUsingFlag { get; }
        private bool rearDoorUsingFlag { get; }
        private int guidanceTime;

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            if (e.Key == Key.T && DisplayType != DISPLAY_TYPE.SEATMAPS)
            {
                if (timer != null && timer.Enabled) timer.Enabled = false;
                seatMapPage = new SeatMapPage(aircraftConfigurationId, frontDoorUsingFlag, rearDoorUsingFlag, "C_3");
                DisplayFrame.Navigate(seatMapPage);
                timer = new Timer(guidanceTime * 1000);
                timer.AutoReset = false;
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
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
            });
        }

        private async void GetGuidanceTime()
        {
            var airlineRef = await new DBConnector().GetDataRow("AirlineReference",
                new DataRow("AirlineCode", Application.Current.Resources["AirlineCode"]));
            if (airlineRef.HasData && airlineRef.Error == ERROR.NoError)
                guidanceTime = (int) airlineRef.Get("ShowGuidanceInSeconds");
            else guidanceTime = 10;
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