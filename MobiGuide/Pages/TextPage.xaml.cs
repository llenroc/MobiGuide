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
using DatabaseConnector;
using CustomExtensions;
using Properties;
using System.Timers;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for TextPage.xaml
    /// </summary>
    public partial class TextPage : Page
    {
        private ShowText ShowText { get; set; }
        private static Timer timer;
        private string displayText;
        public TextPage()
        {
            InitializeComponent();
        }
        public TextPage(ShowText showText) : this()
        {
            this.ShowText = showText;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            double width = this.ActualWidth;
            double height = this.ActualHeight;
            bool IsHorizontal = width > height ? true : false;
            double textWidth = width * (IsHorizontal ? (15f / 16f) : (8f / 9f));
            double textHeight = height * (IsHorizontal ? (4f / 9f) : (7f / 16f));
            double textMarginTop = height / 2;
            textBlock.Width = textWidth;
            textBlock.Height = textHeight;
            textBlock.Margin = new Thickness(0, textMarginTop, 0, 0);

            logoImg.Height = IsHorizontal ? (4f / 9f) * height : (6f / 16f) * height;
            logoImg.Width = logoImg.Height;
            logoImg.Margin = new Thickness(0, 0, 0, (IsHorizontal ? (0.25f / 9f * height) : (1f / 16f * height)) + (height / 2));

            ApplyStyle();
            DisplayLogo();
            DisplayText();
        }

        private async void ApplyStyle()
        {
            DataRow airlineRef = await(new DBConnector()).GetDataRow("AirlineReference", new DataRow("AirlineCode", Application.Current.Resources["AirlineCode"]));
            if (airlineRef.HasData && airlineRef.Error == ERROR.NoError)
            {
                if (airlineRef.ContainKey("BackGroundColor"))
                {
                    try
                    {
                        mainGrid.Background = new SolidColorBrush(((int)airlineRef.Get("BackGroundColor")).GetColor());
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Can not Get Background Color", "ERROR");
                    }
                }
                if (airlineRef.ContainKey("FontColor"))
                {
                    try
                    {
                        textBlock.Foreground = new SolidColorBrush(((int)airlineRef.Get("FontColor")).GetColor());
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Can not Get Font Color", "ERROR");
                    }
                }
                if (airlineRef.ContainKey("FontName"))
                {
                    try
                    {
                        textBlock.FontFamily = new FontFamily(airlineRef.Get("FontName").ToString());
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Can not Get Font Name", "ERROR");
                    }
                }
                if (airlineRef.ContainKey("FontSize"))
                {
                    try
                    {
                        textBlock.FontSize = (int)airlineRef.Get("FontSize");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Can not Get Font Size", "ERROR");
                    }
                }
            }
        }

        private async void DisplayLogo()
        {
            DataRow airlineRef = await (new DBConnector()).GetDataRow("AirlineReference", new DataRow("AirlineCode", Application.Current.Resources["AirlineCode"]));
            if(airlineRef.HasData && airlineRef.Error == ERROR.NoError)
            {
                if (airlineRef.ContainKey("AirlineLogoLarge"))
                {
                    try
                    {
                        logoImg.Source = ((object)airlineRef.Get("AirlineLogoLarge")).BlobToSource();
                    } catch (Exception)
                    {
                        MessageBox.Show("Can not display Airline Logo", "ERROR");
                    }
                }
            }
        }

        private void DisplayText()
        {
            if(ShowText != null)
            {
                displayText = ShowText.TextTemplate;
                Dictionary<string, string> toReplaceList = new Dictionary<string, string>();
                toReplaceList.Add("[flight]", ShowText.FlightNo);
                toReplaceList.Add("[origin]", String.Format("{0} ({1})", ShowText.OriginName, ShowText.OriginCode));
                toReplaceList.Add("[dest]", String.Format("{0} ({1})", ShowText.DestinationName, ShowText.DestinationCode));
                toReplaceList.Add("[depttime]", ShowText.DepartureTime.ToString(@"hh\:mm"));
                toReplaceList.Add("[deptdate]", ShowText.DepartureDate.ToString(@"hh\:mm"));
                //int boarding_min = (ShowText.DepartureTime - DateTime.Now.TimeOfDay).Minutes;
                //toReplaceList.Add("[boarding_min]", boarding_min.ToString() + " Minute" + (boarding_min == 1 ? "" : "s"));

                foreach (KeyValuePair<string, string> toReplace in toReplaceList)
                {
                    if (displayText.Contains(toReplace.Key))
                        displayText = displayText.Replace(toReplace.Key, toReplace.Value);
                }

                SetText();

                if (displayText.Contains("[boarding_min]"))
                {
                    timer = new System.Timers.Timer();
                    timer.AutoReset = false;
                    timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
                    timer.Interval = GetInterval();
                    timer.Start();
                }
            }
        }

        double GetInterval()
        {
            DateTime now = DateTime.Now;
            return ((60 - now.Second) * 1000 - now.Millisecond);
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                SetText();
            }));
            timer.Interval = GetInterval();
            timer.Start();
        }

        private void SetText()
        {
            int boarding_min = (int)(ShowText.DepartureTime - DateTime.Now.TimeOfDay).TotalMinutes;
            boarding_min = boarding_min > 0 ? boarding_min : 0;
            textBlock.Text = displayText.Replace("[boarding_min]", boarding_min.ToString() + " Minute" + (boarding_min > 1 ? "s" : ""));
        }
    }
}
