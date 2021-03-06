﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DatabaseConnector;
using CustomExtensions;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for LogoPage.xaml
    /// </summary>
    public partial class LogoPage : Page
    {
        public LogoPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            double width = ActualWidth;
            double height = ActualHeight;
            double logoImgSize = (width < height ? width : height) * 34 / 55; // golden ratio
            logoImg.Height = logoImgSize;
            logoImg.Width = logoImgSize;

            LoadLogo();
        }

        private async void LoadLogo()
        {
            DBConnector dbCon = new DBConnector();
            DataRow airlineReferenceData = await dbCon.GetDataRow("AirlineReference", new DataRow("AirlineCode", Application.Current.Resources["AirlineCode"]));
            if(airlineReferenceData.HasData && airlineReferenceData.Error == ERROR.NoError)
            {
                logoImg.Source = airlineReferenceData.Get("AirlineLogoLarge").BlobToSource();
                if(logoImg.Source == null) logoImg.Source = new BitmapImage(new Uri(@"..\NoImg.jpg", UriKind.RelativeOrAbsolute));
                if (airlineReferenceData.Get("BackGroundColor2") != null)
                {
                    Color backgroundColor2 = ((int)airlineReferenceData.Get("BackGroundColor2")).GetColor();
                    if (backgroundColor2.A != 0)
                    {
                        LinearGradientBrush gradiantBrush =
                        new LinearGradientBrush();
                        gradiantBrush.StartPoint = new Point(0, 0);
                        gradiantBrush.EndPoint = new Point(1, 1);
                        gradiantBrush.GradientStops.Add(
                            new GradientStop(((int)airlineReferenceData.Get("BackGroundColor")).GetColor(), 0.3));
                        gradiantBrush.GradientStops.Add(
                            new GradientStop(backgroundColor2, 1.0));
                        logoGrid.Background = gradiantBrush;
                    } else
                    {
                        logoGrid.Background = new SolidColorBrush(((int)airlineReferenceData.Get("BackGroundColor")).GetColor());
                    }
                } else
                {
                    logoGrid.Background = new SolidColorBrush(((int)airlineReferenceData.Get("BackGroundColor")).GetColor());
                }
            } else
            {
                logoImg.Source = new BitmapImage(new Uri(@"..\NoImg.jpg", UriKind.RelativeOrAbsolute));
            }
        }
    }
}
