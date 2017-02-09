﻿using System;
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
using System.Diagnostics;
using System.Reflection;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for TextPage.xaml
    /// </summary>
    public partial class TextPage : Page
    {
        private ShowText ShowText { get; set; }
        private List<TextTranslation> TextTranslationList { get; set; }
        private List<AirportTranslation> OriginAirportTranslationList { get; set; }
        private List<AirportTranslation> DestinationAirportTranslationList { get; set; }
        private static Timer minuteTimer;
        private static Timer rotateTimer;
        private string displayText;
        private string currentLanguage { get; set; }
        private int indexOfTranslation { get; set; }
        DBConnector dbCon = new DBConnector();
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
            DataRow airlineRef = await dbCon.GetDataRow("AirlineReference", new DataRow("AirlineCode", Application.Current.Resources["AirlineCode"]));
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

        private async void DisplayText()
        {
            if(ShowText != null)
            {
                indexOfTranslation = 0;
                TextTranslationList = new List<TextTranslation>();
                TextTranslationList.Add(new TextTranslation()
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
            Dispatcher.Invoke(new Action(() =>
            {
                DisplayMessage(currentLanguage);
            }));
            minuteTimer.Interval = GetInterval();
            minuteTimer.Start();
        }

        private string ReplaceTextTemplate(string str, string languageCode)
        {
            currentLanguage = languageCode;
            string replacedText = str;
            Dictionary<string, string> toReplaceList = new Dictionary<string, string>();
            toReplaceList.Add("[flight]", ShowText.FlightNo);
            toReplaceList.Add("[origin]", String.Format("{0} ({1})", 
                (languageCode == "ENG" || OriginAirportTranslationList.Find(item => item.LanguageCode == languageCode) == null) ? ShowText.OriginName : OriginAirportTranslationList.Find(item => item.LanguageCode == languageCode).AirportName,  
                ShowText.OriginCode));
            toReplaceList.Add("[dest]", String.Format("{0} ({1})", 
                (languageCode == "ENG" || DestinationAirportTranslationList.Find(item => item.LanguageCode == languageCode) == null) ? ShowText.DestinationName : DestinationAirportTranslationList.Find(item => item.LanguageCode == languageCode).AirportName, 
                ShowText.DestinationCode));
            toReplaceList.Add("[depttime]", ShowText.DepartureTime.
                ToString(@"hh\:mm"));
            toReplaceList.Add("[deptdate]", ShowText.DepartureDate.ToString(@"MM\\dd\\yyyy"));

            foreach (KeyValuePair<string, string> toReplace in toReplaceList)
            {
                if (str.Contains(toReplace.Key))
                    str = str.Replace(toReplace.Key, toReplace.Value);
            }

            if (str.Contains("[boarding_min]"))
            {
                str = SetRemainBoardingTime(str);
            }
            if (str.Contains("[boarding_day]"))
            {
                str = SetRemainBoardingDay(str);
            }

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
                    {
                        if (textTranslationRow.ContainKey(property.Name))
                        {
                            property.SetValue(textTranslation, textTranslationRow.Get(property.Name));
                        }
                    }
                    TextTranslationList.Add(textTranslation);
                }
                foreach(TextTranslation textTranslation in TextTranslationList)
                {
                    DataRow originAirportTranslation = await dbCon.GetDataRow("AirportTranslation", 
                        new DatabaseConnector.DataRow("AirportCode", ShowText.OriginCode, "LanguageCode", textTranslation.LanguageCode));
                    if (originAirportTranslation.HasData && originAirportTranslation.Error == ERROR.NoError)
                    {
                        AirportTranslation airportTranslation = new AirportTranslation();
                        airportTranslation = AddDataToProperty(typeof(AirportTranslation), originAirportTranslation) as AirportTranslation;
                        OriginAirportTranslationList.Add(airportTranslation);
                    }

                    DataRow destinationAirportTranslation = await dbCon.GetDataRow("AirportTranslation",
                        new DatabaseConnector.DataRow("AirportCode", ShowText.DestinationCode, "LanguageCode", textTranslation.LanguageCode));
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
            {
                if (data.ContainKey(property.Name))
                {
                    property.SetValue(instance, data.Get(property.Name));
                }
            }
            return instance;
        }

        private void RotateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                displayText = TextTranslationList[indexOfTranslation].TextTemplate;
                DisplayMessage(TextTranslationList[indexOfTranslation].LanguageCode);
                indexOfTranslation++;
                if (indexOfTranslation >= TextTranslationList.Count) indexOfTranslation = 0;
            }));
            rotateTimer.Interval = ShowText.RotateInSeconds * 1000;
            rotateTimer.Start();
        }

        private void DisplayMessage(string languageCode)
        {
            textBlock.Text = ReplaceTextTemplate(displayText, languageCode);
        }

        double GetInterval()
        {
            DateTime now = DateTime.Now;
            return ((60 - now.Second) * 1000 - now.Millisecond);
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