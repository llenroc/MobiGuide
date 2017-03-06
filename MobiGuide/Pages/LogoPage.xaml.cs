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
            double width = this.ActualWidth;
            double height = this.ActualHeight;
            double logoImgSize = (width < height ? width : height) * 34 / 55; // golden ratio
            logoImg.Height = logoImgSize;
            logoImg.Width = logoImgSize;

            LoadLogo();
        }

        private async void LoadLogo()
        {
            DBConnector dbCon = new DBConnector();
            DataRow airlineReferenceData = await dbCon.GetDataRow("AirlineReference", new DatabaseConnector.DataRow("AirlineCode", Application.Current.Resources["AirlineCode"]));
            if(airlineReferenceData.HasData && airlineReferenceData.Error == ERROR.NoError)
            {
                logoImg.Source = (airlineReferenceData.Get("AirlineLogoLarge") as object).BlobToSource();
                if(airlineReferenceData.Get("BackGroundColor2") != null)
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
                logoImg.Source = new BitmapImage(new Uri(@"NoImg.jpg", UriKind.RelativeOrAbsolute));
            }
        }
    }
}
