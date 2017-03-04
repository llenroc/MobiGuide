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
using Properties;
using System.Reflection;
using CustomExtensions;
using System.Timers;
using MobiGuide;
using System.Diagnostics;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for SeatMapPage.xaml
    /// </summary>
    public partial class SeatMapPage : Page
    {
        DBConnector dbCon = new DBConnector();
        private Guid AircraftConfigurationId { get; set; }
        private bool FrontDoorUsingFlag { get; set; }
        private bool RearDoorUsingFlag { get; set; }
        private AircraftConfiguration AircraftConfiguration { get; set; }
        private List<Seat> SeatMapPostion { get; set; }
        private double AisleXWidth { get; set; }
        private SolidColorBrush lineBrush { get; set; }
        private SolidColorBrush seatBrush { get; set; }
        private SolidColorBrush fontBrush { get; set; }
        private Timer DisplayGuidanceTimer;
        private int DisplayGuidanceTime { get; set; }
        private bool isFlipped { get; set; }
        private string selectedSeat { get; set; }
        public SeatMapPage()
        {
            InitializeComponent();
        }

        public SeatMapPage(Guid aircraftConfigId, bool frontDoorUsingFlag, bool rearDoorUsingFlag) : this()
        {
            if (aircraftConfigId != Guid.Empty)
                AircraftConfigurationId = aircraftConfigId;
            FrontDoorUsingFlag = frontDoorUsingFlag;
            RearDoorUsingFlag = rearDoorUsingFlag;
        }

        public SeatMapPage(Guid aircraftConfigId, bool frontDoorUsingFlag, bool rearDoorUsingFlag, string seatName) : this(aircraftConfigId, frontDoorUsingFlag, rearDoorUsingFlag)
        {
            this.selectedSeat = seatName;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UIElementSizeAdjust();
            await LoadAirlineReferenceData();
            await LoadSeatMapImage();
            await LoadSeatMapPosition();
            isFlipped = false;
        }

        private void UIElementSizeAdjust()
        {
            seatMapImg.Height = imageCanvas.ActualHeight;
            seatMapImg.Width = seatMapImg.Height * 9f / 16f;
            imageCanvas.Height = seatMapImg.Height;
            imageCanvas.Width = seatMapImg.Width;
            seatNumberViewBox.Width = this.ActualWidth > this.ActualHeight ? this.ActualWidth / 9f : this.ActualWidth / 5f;
        }

        private async Task LoadAirlineReferenceData()
        {
            DataRow airlineRef = await dbCon.GetDataRow("AirlineReference", new DataRow("AirlineCode", Application.Current.Resources["AirlineCode"]));
            if(airlineRef.HasData && airlineRef.Error == ERROR.NoError)
            {
                lineBrush = airlineRef.ContainKey("LineColor") ? new SolidColorBrush(((int)airlineRef.Get("LineColor")).GetColor()) : Brushes.Red;
                seatBrush = airlineRef.ContainKey("SeatColor") ? new SolidColorBrush(((int)airlineRef.Get("SeatColor")).GetColor()) : Brushes.Red;
                DisplayGuidanceTime = airlineRef.ContainKey("ShowGuidanceInSeconds") ? (int)airlineRef.Get("ShowGuidanceInSeconds") : 10;
            }
            (seatNumberViewBox.Child as TextBlock).Foreground = seatBrush;
        }

        private void DrawSeat()
        {
            seatComboBox.Items.Add(new CustomComboBoxItem { Text = "Not Selected", Value = ""});
            foreach(Seat seat in SeatMapPostion)
            {
                Grid grid = new Grid();
                grid.Width = seat.Width;
                grid.Height = seat.Height;
                grid.Background = seatBrush;
                grid.Visibility = Visibility.Hidden;
                TextBlock text = new TextBlock();
                text.Text = String.Format("{0}{1}", seat.Row, seat.Column);
                text.Foreground = Brightness(seatBrush.Color) < 130 ? Brushes.White : Brushes.Black;
                if ((FrontDoorUsingFlag && !RearDoorUsingFlag) || (FrontDoorUsingFlag && RearDoorUsingFlag && (seat.Row < this.AircraftConfiguration.MiddleRow))) text.LayoutTransform = new RotateTransform(180);
                Viewbox viewBox = new Viewbox();
                viewBox.Child = text;
                grid.Name = String.Format("{1}_{0}", seat.Row, seat.Column);
                this.RegisterName(String.Format("{1}_{0}", seat.Row, seat.Column), grid);
                grid.Children.Add(viewBox);
                imageCanvas.Children.Add(grid);
                Canvas.SetLeft(grid, seat.X);
                Canvas.SetTop(grid, seat.Y);

                seatComboBox.Items.Add(new CustomComboBoxItem { Text = String.Format("{0}{1}", seat.Row, seat.Column), Value = String.Format("{1}_{0}", seat.Row, seat.Column) });
            }
            seatComboBox.SelectedIndex = 0;

            if (this.selectedSeat != null)
            {
                ShowSeat(this.selectedSeat);
                DrawLine(this.selectedSeat);
            }
        }

        private void DrawLine(string seatName)
        {
            int seatRow = Int32.Parse(seatName.Split('_')[1]);
            string seatColumn = seatName.Split('_')[0];
            Seat seat = SeatMapPostion.Find(item => (item.Row == seatRow) && (item.Column == seatColumn));
            (seatNumberViewBox.Child as TextBlock).Text = String.Format("{0}{1}", seatRow, seatColumn);
            List<UIElement> toRemove = new List<UIElement>();
            foreach (UIElement child in imageCanvas.Children)
            {
                if(child is Line || child is Polygon)
                {
                    toRemove.Add(child);
                }
            }

            foreach (UIElement line in toRemove)
            {
                imageCanvas.Children.Remove(line);
            }


            double startPositionX = -1;
            double startPositionY = -1;

            if(FrontDoorUsingFlag && RearDoorUsingFlag)
            {
                startPositionX = seatRow < this.AircraftConfiguration.MiddleRow ? this.AircraftConfiguration.FrontDoorX : this.AircraftConfiguration.RearDoorX;
                startPositionY = seatRow < this.AircraftConfiguration.MiddleRow ? this.AircraftConfiguration.FrontDoorY : this.AircraftConfiguration.RearDoorY;
            } else
            {
                startPositionX = FrontDoorUsingFlag ? this.AircraftConfiguration.FrontDoorX : this.AircraftConfiguration.RearDoorX;
                startPositionY = FrontDoorUsingFlag ? this.AircraftConfiguration.FrontDoorY : this.AircraftConfiguration.RearDoorY;
            }

            startPositionX -= 50;

            Debug.WriteLine("Start X : " + startPositionX);
            Debug.WriteLine("Start Y : " + startPositionY);

            Debug.WriteLine("--------------------------");

            Line doorToAisleXLine = new Line();
            doorToAisleXLine.X1 = startPositionX;
            doorToAisleXLine.Y1 = startPositionY;
            doorToAisleXLine.X2 = this.AircraftConfiguration.AisleX;
            doorToAisleXLine.Y2 = startPositionY;
            doorToAisleXLine.StrokeThickness = AisleXWidth;
            doorToAisleXLine.Stroke = lineBrush;

            Line aisleLine = new Line();
            aisleLine.X1 = this.AircraftConfiguration.AisleX;
            aisleLine.Y1 = (FrontDoorUsingFlag && !RearDoorUsingFlag) || (FrontDoorUsingFlag && RearDoorUsingFlag && (seatRow < this.AircraftConfiguration.MiddleRow)) ? startPositionY - (AisleXWidth / 2) : startPositionY + (AisleXWidth / 2);
            aisleLine.X2 = this.AircraftConfiguration.AisleX;
            aisleLine.Y2 = seat.Y + (seat.Height / 2);
            aisleLine.StrokeThickness = AisleXWidth;
            aisleLine.Stroke = lineBrush;

            Line aisleToSeatLine = new Line();
            aisleToSeatLine.X1 = this.AircraftConfiguration.AisleX < seat.X ? this.AircraftConfiguration.AisleX - (AisleXWidth / 2) : this.AircraftConfiguration.AisleX + (AisleXWidth / 2);
            aisleToSeatLine.Y1 = seat.Y + (seat.Height / 2);
            aisleToSeatLine.X2 = this.AircraftConfiguration.AisleX < seat.X ? seat.X - seat.Width : seat.X + (seat.Width * 2);
            aisleToSeatLine.Y2 = seat.Y + (seat.Height / 2);
            aisleToSeatLine.StrokeThickness = AisleXWidth;
            aisleToSeatLine.Stroke = lineBrush;

            PointCollection pointCollection = new PointCollection();
            pointCollection.Add(new Point(aisleToSeatLine.X2, aisleToSeatLine.Y2 + AisleXWidth));
            pointCollection.Add(new Point(aisleToSeatLine.X2, aisleToSeatLine.Y2 - AisleXWidth));
            pointCollection.Add(new Point(this.AircraftConfiguration.AisleX < seat.X ? aisleToSeatLine.X2 + seat.Width : aisleToSeatLine.X2 - seat.Width, aisleToSeatLine.Y2));

            Polygon arrow = new Polygon();
            arrow.Points = pointCollection;
            arrow.Fill = lineBrush;
            arrow.StrokeThickness = 1;
            arrow.Stroke = Brushes.Black;

            imageCanvas.Children.Add(doorToAisleXLine);
            imageCanvas.Children.Add(aisleLine);
            imageCanvas.Children.Add(aisleToSeatLine);
            imageCanvas.Children.Add(arrow);

            if (seatRow >= this.AircraftConfiguration.MiddleRow && (FrontDoorUsingFlag && RearDoorUsingFlag))
            {
                imageCanvas.LayoutTransform = new RotateTransform(0);
                isFlipped = true;
            }

            if (DisplayGuidanceTimer != null && DisplayGuidanceTimer.Enabled) DisplayGuidanceTimer.Enabled = false;
            DisplayGuidanceTimer = new Timer(DisplayGuidanceTime * 1000);
            DisplayGuidanceTimer.AutoReset = false;
            DisplayGuidanceTimer.Elapsed += DisplayGuidanceTimer_Elapsed;
            DisplayGuidanceTimer.Start();
        }

        private void DisplayGuidanceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                seatComboBox.SelectedIndex = 0;
                HideSeatAndLine();
                if (isFlipped)
                {
                    imageCanvas.LayoutTransform = new RotateTransform(180);
                    isFlipped = false;
                }
            }));
        }

        private static int Brightness(Color c)
        {
            return (int)Math.Sqrt(
               c.R * c.R * .241 +
               c.G * c.G * .691 +
               c.B * c.B * .068);
        }

        private async Task LoadSeatMapPosition()
        {
            DataList seatMapDataList = await dbCon.GetDataList("SeatMap", new DataRow("AircraftConfigurationId", this.AircraftConfigurationId), "ORDER BY SeatRow, SeatColumn");
            if(seatMapDataList.HasData && seatMapDataList.Error == ERROR.NoError)
            {
                SeatMapPostion = new List<Seat>();
                foreach(DataRow seatMapData in seatMapDataList)
                {
                    Seat seat = new Seat()
                    {
                        X = AdjustRatio((double)seatMapData.Get("PositionX")),
                        Y = AdjustRatio((double)seatMapData.Get("PositionY"), false),
                        Width = AdjustRatio((double)seatMapData.Get("SeatWidth")),
                        Height = AdjustRatio((double)seatMapData.Get("SeatHeight"), false),
                        Row = (int)seatMapData.Get("SeatRow"),
                        Column = seatMapData.Get("SeatColumn").ToString()
                    };
                    SeatMapPostion.Add(seat);
                }

                AisleXWidth = (SeatMapPostion[3].X - (SeatMapPostion[2].X + SeatMapPostion[2].Width)) / 2;

                DrawSeat();
            }
        }

        private double AdjustRatio(double oldVal, bool isWidth = true)
        {
            return isWidth ? seatMapImg.Width * oldVal / 1080 : seatMapImg.Height * oldVal / 1920;
        }

        private async Task LoadSeatMapImage()
        {
            DataRow aircraftConfigData = await dbCon.GetDataRow("AircraftConfiguration", new DatabaseConnector.DataRow("AircraftConfigurationId", AircraftConfigurationId));
            if(aircraftConfigData.HasData && aircraftConfigData.Error == ERROR.NoError)
            {
                this.AircraftConfiguration = new AircraftConfiguration();
                PropertyInfo[] properties = typeof(AircraftConfiguration).GetProperties();
                foreach(PropertyInfo property in properties)
                {
                    if (aircraftConfigData.ContainKey(property.Name))
                    {
                        property.SetValue(this.AircraftConfiguration, aircraftConfigData.Get(property.Name));
                    }
                }

                DataRow aircraftTypeData = await dbCon.GetDataRow("AircraftTypeReference", new DataRow("AircraftTypeCode", aircraftConfigData.Get("AircraftTypeCode")));
                if (aircraftTypeData.HasData && aircraftTypeData.Error == ERROR.NoError)
                {
                    AircraftType aircraftType = new AircraftType();
                    properties = typeof(AircraftType).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        if (aircraftTypeData.ContainKey(property.Name))
                        {
                            property.SetValue(aircraftType, aircraftTypeData.Get(property.Name));
                        }
                    }
                    this.AircraftConfiguration.AircraftType = aircraftType;
                }

                double AisleX = AdjustRatio(this.AircraftConfiguration.AisleX);
                double FrontDoorX = AdjustRatio(this.AircraftConfiguration.FrontDoorX);
                double FrontDoorY = AdjustRatio(this.AircraftConfiguration.FrontDoorY, false);
                double FrontDoorWidth = AdjustRatio(this.AircraftConfiguration.FrontDoorWidth, false);

                double RearDoorX = AdjustRatio(this.AircraftConfiguration.RearDoorX);
                double RearDoorY = AdjustRatio(this.AircraftConfiguration.RearDoorY, false);
                double RearDoorWidth = AdjustRatio(this.AircraftConfiguration.RearDoorWidth, false);

                this.AircraftConfiguration.AisleX = AisleX;
                this.AircraftConfiguration.FrontDoorX = FrontDoorX;
                this.AircraftConfiguration.FrontDoorY = FrontDoorY;
                this.AircraftConfiguration.FrontDoorWidth = FrontDoorWidth;

                this.AircraftConfiguration.RearDoorX = RearDoorX;
                this.AircraftConfiguration.RearDoorY = RearDoorY;
                this.AircraftConfiguration.RearDoorWidth = RearDoorWidth;

                seatMapImg.Source = ((object)aircraftConfigData.Get("SeatMapImage")).BlobToSource();
                if (this.selectedSeat != null && FrontDoorUsingFlag && RearDoorUsingFlag)
                {
                    int selectedRow = Int32.Parse(this.selectedSeat.Split('_')[1]);
                    if (selectedRow < this.AircraftConfiguration.MiddleRow)
                        imageCanvas.LayoutTransform = new RotateTransform(180);
                }
                else if (FrontDoorUsingFlag)
                {
                    imageCanvas.LayoutTransform = new RotateTransform(180);
                }
            }
        }

        private void ShowSeat(string selectedSeat)
        {
            foreach (UIElement child in imageCanvas.Children)
            {
                if (child is Grid)
                {
                    Grid grid = child as Grid;
                    grid.Visibility = grid.Name == selectedSeat ? Visibility.Visible : Visibility.Hidden;
                }
            }
        }

        private void HideSeatAndLine()
        {
            List<UIElement> toRemoveList = new List<UIElement>();
            foreach (UIElement child in imageCanvas.Children)
            {
                if (child is Grid)
                {
                    Grid grid = child as Grid;
                    grid.Visibility = Visibility.Hidden;
                }
                if (child is Line || child is Polygon)
                {
                    toRemoveList.Add(child);
                }
            }
            foreach (UIElement toRemove in toRemoveList)
            {
                imageCanvas.Children.Remove(toRemove);
            }
                (seatNumberViewBox.Child as TextBlock).Text = String.Empty;
        }

        private void seatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (seatComboBox.SelectedItem != null && seatComboBox.SelectedValue.ToString() != "")
                {
                    string selectedSeat = (seatComboBox.SelectedItem as CustomComboBoxItem).Value.ToString();
                    ShowSeat(selectedSeat);
                    DrawLine(selectedSeat);
                }
                else
                {
                    HideSeatAndLine();
                }
            } catch (Exception) {
            }
        }

        private void extractBtn_Click(object sender, RoutedEventArgs e)
        {
            string barcode = barCodeTextBox.Text;
            BarcodeExtractor barcodeExtractor = new BarcodeExtractor(barcode);
            string seatName = barcodeExtractor.GetSeatColumn() + "_" + barcodeExtractor.GetSeatRow();
            ShowSeat(seatName);
            DrawLine(seatName);
        }
    }
}
