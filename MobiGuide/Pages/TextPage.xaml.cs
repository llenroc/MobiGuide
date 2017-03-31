using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DatabaseConnector;
using CustomExtensions;
using Properties;
using System.Timers;
using System.Reflection;
using MobiGuide.Class;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for TextPage.xaml
    /// </summary>
    public partial class TextPage : Page
    {
        private static Timer minuteTimer;
        private static Timer rotateTimer;
        private readonly DBConnector dbCon = new DBConnector();
        private string displayText;

        public TextPage()
        {
            InitializeComponent();
        }

        public TextPage(ShowText showText) : this()
        {
            ShowText = showText;
        }

        private ShowText ShowText { get; }
        private List<TextTranslation> TextTranslationList;
        private List<AirportTranslation> OriginAirportTranslationList;
        private List<AirportTranslation> DestinationAirportTranslationList;
        private string currentLanguage;
        private int indexOfTranslation;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            double width = ActualWidth;
            double height = ActualHeight;
            bool IsHorizontal = width > height ? true : false;
            double textWidth = width * (IsHorizontal ? 15f / 16f : 8f / 9f);
            double textHeight = height * (IsHorizontal ? 4f / 9f : 7f / 16f);
            double textMarginTop = height / 2;
            textBlock.Width = textWidth;
            textBlock.Height = textHeight;
            textBlock.Margin = new Thickness(0, textMarginTop, 0, 0);

            logoImg.Height = IsHorizontal ? 4f / 9f * height : 6f / 16f * height;
            logoImg.Width = logoImg.Height;
            logoImg.Margin = new Thickness(0, 0, 0, (IsHorizontal ? 0.25f / 9f * height : 1f / 16f * height) + height / 2);

            ApplyStyle();
            DisplayLogo();
            DisplayText();
        }

        private async void ApplyStyle()
        {
            DataRow airlineRef = await new DBConnector().GetDataRow("AirlineReference", new DataRow("AirlineCode", Application.Current.Resources["AirlineCode"]));
            if (airlineRef.HasData && airlineRef.Error == ERROR.NoError)
            {
                if (airlineRef.ContainKey("BackGroundColor"))
                    try
                    {
                        if (airlineRef.Get("BackGroundColor2") != null)
                        {
                            Color backgroundColor2 = ((int)airlineRef.Get("BackGroundColor2")).GetColor();
                            if (backgroundColor2.A != 0)
                            {
                                LinearGradientBrush gradiantBrush =
                                    new LinearGradientBrush();
                                gradiantBrush.StartPoint = new Point(0, 0);
                                gradiantBrush.EndPoint = new Point(1, 1);
                                gradiantBrush.GradientStops.Add(
                                    new GradientStop(((int)airlineRef.Get("BackGroundColor")).GetColor(), 0.3));
                                gradiantBrush.GradientStops.Add(
                                    new GradientStop(backgroundColor2, 1.0));
                                mainGrid.Background = gradiantBrush;
                            }
                            else
                            {
                                mainGrid.Background = new SolidColorBrush(((int)airlineRef.Get("BackGroundColor")).GetColor());
                            }
                        }
                        else
                        {
                            mainGrid.Background = new SolidColorBrush(((int)airlineRef.Get("BackGroundColor")).GetColor());
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(Messages.ERROR_GET_BG_COLOR, Captions.ERROR);
                    }
                if (airlineRef.ContainKey("FontColor"))
                    try
                    {
                        textBlock.Foreground = new SolidColorBrush(((int)airlineRef.Get("FontColor")).GetColor());
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(Messages.ERROR_GET_FONT_COLOR, Captions.ERROR);
                    }
                if (airlineRef.ContainKey("FontName"))
                    try
                    {
                        textBlock.FontFamily = new FontFamily(airlineRef.Get("FontName").ToString());
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(Messages.ERROR_GET_FONT_NAME, Captions.ERROR);
                    }
                if (airlineRef.ContainKey("FontSize"))
                    try
                    {
                        textBlock.FontSize = (int)airlineRef.Get("FontSize");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(Messages.ERROR_GET_FONT_SIZE, Captions.ERROR);
                    }
            }
        }

        private async void DisplayLogo()
        {
            DataRow airlineRef = await dbCon.GetDataRow("AirlineReference", new DataRow("AirlineCode", Application.Current.Resources["AirlineCode"]));
            if(airlineRef.HasData && airlineRef.Error == ERROR.NoError)
                if (airlineRef.ContainKey("AirlineLogoLarge"))
                    try
                    {
                        logoImg.Source = airlineRef.Get("AirlineLogoLarge").BlobToSource();
                        if (logoImg.Source == null) logoImg.Source = new BitmapImage(new Uri(@"..\NoImg.jpg", UriKind.RelativeOrAbsolute));
                    } catch (Exception)
                    {
                        MessageBox.Show(Messages.ERROR_SHOW_AIRLINE_LOGO, Captions.ERROR);
                    }
        }

        private async void DisplayText()
        {
            if(ShowText != null)
            {
                indexOfTranslation = 0;
                TextTranslationList = new List<TextTranslation>();
                TextTranslationList.Add(new TextTranslation
                {
                    LanguageCode = "ENG",
                    TextTemplate = ShowText.TextTemplate
                });
                OriginAirportTranslationList = new List<AirportTranslation>();
                DestinationAirportTranslationList = new List<AirportTranslation>();
                displayText = ShowText.TextTemplate;
                await InitializeRotation();
                InitializeMinuteTimer();
            }
        }

        private void InitializeMinuteTimer()
        {
            minuteTimer = new Timer();
            minuteTimer.AutoReset = false;
            minuteTimer.Elapsed += MinuteTimer_Elapsed;
            minuteTimer.Interval = GetInterval();
            minuteTimer.Start();
        }

        private void MinuteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                DisplayMessage(currentLanguage);
            });
            minuteTimer.Interval = GetInterval();
            minuteTimer.Start();
        }

        private string ReplaceTextTemplate(string str, string languageCode)
        {
            currentLanguage = languageCode;
            string replacedText = str;
            Dictionary<string, string> toReplaceList = new Dictionary<string, string>();
            toReplaceList.Add("[flight]", ShowText.FlightNo);
            toReplaceList.Add("[origin]", string.Format("{0} ({1})", 
                languageCode == "ENG" || OriginAirportTranslationList.Find(item => item.LanguageCode == languageCode) == null ? ShowText.OriginName : OriginAirportTranslationList.Find(item => item.LanguageCode == languageCode).AirportName,  
                ShowText.OriginCode));
            toReplaceList.Add("[dest]", string.Format("{0} ({1})", 
                languageCode == "ENG" || DestinationAirportTranslationList.Find(item => item.LanguageCode == languageCode) == null ? ShowText.DestinationName : DestinationAirportTranslationList.Find(item => item.LanguageCode == languageCode).AirportName, 
                ShowText.DestinationCode));
            toReplaceList.Add("[depttime]", ShowText.DepartureTime.
                ToString(@"hh\:mm"));
            toReplaceList.Add("[deptdate]", ShowText.DepartureDate.ToString(@"MM\\dd\\yyyy"));

            foreach (KeyValuePair<string, string> toReplace in toReplaceList)
                if (str.Contains(toReplace.Key))
                    str = str.Replace(toReplace.Key, toReplace.Value);

            if (str.Contains("[boarding_min]"))
                str = SetRemainBoardingTime(str);
            if (str.Contains("[boarding_day]"))
                str = SetRemainBoardingDay(str);

            return str;
        }

        private async Task InitializeRotation()
        {
            DisplayMessage("ENG");
            rotateTimer = new Timer(ShowText.RotateInSeconds * 1000);
            rotateTimer.AutoReset = false;
            rotateTimer.Elapsed += RotateTimer_Elapsed;
            rotateTimer.Start();
            DataList textTranslationList = await dbCon.GetDataList("TextTranslation", new DataRow("TextTemplateId", ShowText.TextTemplateId));
            if(textTranslationList.HasData && textTranslationList.Error == ERROR.NoError)
            {
                foreach(DataRow textTranslationRow in textTranslationList)
                {
                    TextTranslation textTranslation = new TextTranslation();
                    PropertyInfo[] properties = textTranslation.GetType().GetProperties();
                    foreach(PropertyInfo property in properties)
                        if (textTranslationRow.ContainKey(property.Name))
                            property.SetValue(textTranslation, textTranslationRow.Get(property.Name));
                    TextTranslationList.Add(textTranslation);
                }
                foreach(TextTranslation textTranslation in TextTranslationList)
                {
                    DataRow originAirportTranslation = await dbCon.GetDataRow("AirportTranslation", 
                        new DataRow("AirportCode", ShowText.OriginCode, "LanguageCode", textTranslation.LanguageCode));
                    if (originAirportTranslation.HasData && originAirportTranslation.Error == ERROR.NoError)
                    {
                        AirportTranslation airportTranslation = new AirportTranslation();
                        airportTranslation = AddDataToProperty(typeof(AirportTranslation), originAirportTranslation) as AirportTranslation;
                        OriginAirportTranslationList.Add(airportTranslation);
                    }

                    DataRow destinationAirportTranslation = await dbCon.GetDataRow("AirportTranslation",
                        new DataRow("AirportCode", ShowText.DestinationCode, "LanguageCode", textTranslation.LanguageCode));
                    if (destinationAirportTranslation.HasData && destinationAirportTranslation.Error == ERROR.NoError)
                    {
                        AirportTranslation airportTranslation = new AirportTranslation();
                        airportTranslation = AddDataToProperty(typeof(AirportTranslation), destinationAirportTranslation) as AirportTranslation;
                        DestinationAirportTranslationList.Add(airportTranslation);
                    }
                }
                indexOfTranslation++;
            }
        }

        private object AddDataToProperty(Type typeOfProperty, DataRow data)
        {
            object instance = Activator.CreateInstance(typeOfProperty);
            PropertyInfo[] properties = instance.GetType().GetProperties();
            foreach(PropertyInfo property in properties)
                if (data.ContainKey(property.Name))
                    property.SetValue(instance, data.Get(property.Name));
            return instance;
        }

        private void RotateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                displayText = TextTranslationList[indexOfTranslation].TextTemplate;
                DisplayMessage(TextTranslationList[indexOfTranslation].LanguageCode);
                indexOfTranslation++;
                if (indexOfTranslation >= TextTranslationList.Count) indexOfTranslation = 0;
            });
            rotateTimer.Interval = ShowText.RotateInSeconds * 1000;
            rotateTimer.Start();
        }

        private void DisplayMessage(string languageCode)
        {
            textBlock.Text = ReplaceTextTemplate(displayText, languageCode);
        }

        private double GetInterval()
        {
            DateTime now = DateTime.Now;
            return (60 - now.Second) * 1000 - now.Millisecond;
        }

        private string SetRemainBoardingTime(string str)
        {
            int boarding_min = (int)(ShowText.DepartureTime - DateTime.Now.TimeOfDay).TotalMinutes;
            boarding_min = boarding_min > 0 ? boarding_min : 0;
            str = str.Replace("[boarding_min]", boarding_min.ToString());
            return str;
        }

        private string SetRemainBoardingDay(string str)
        {
            int boarding_day = (int)Math.Ceiling((ShowText.DepartureDate - DateTime.Now).TotalDays);
            boarding_day = boarding_day > 0 ? boarding_day : 0;
            str = str.Replace("[boarding_day]", boarding_day.ToString());
            return str;
        }
    }
}
