using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Properties;
using DatabaseConnector;
using System.Diagnostics;
using CustomExtensions;
using Xceed.Wpf.Toolkit;
using System.Text.RegularExpressions;
using System.Reflection;
using MobiGuide.Class;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for NewEditAircraftConfigurationPage3.xaml
    /// </summary>
    public partial class NewEditAircraftConfigurationPage3 : Page
    {
        private readonly DBConnector dbCon = new DBConnector();
        public NewEditAircraftConfigurationPage3() : this(null, STATUS.NEW) { }

        public NewEditAircraftConfigurationPage3(AircraftConfiguration aircraftConfiguration, STATUS Status)
        {
            InitializeComponent();
            this.Status = Status;
            if(aircraftConfiguration != null)
                AircraftConfiguration = aircraftConfiguration;
        }

        public STATUS Status { get; set; }

        private Window window
        {
            get
            {
                DependencyObject parent = VisualTreeHelper.GetParent(this);
                while (!(parent is Window))
                    parent = VisualTreeHelper.GetParent(parent);
                return parent as Window;
            }
        }

        private AircraftConfiguration AircraftConfiguration { get; }
        private Seat[,] seatPosition { get; set; }
        private string seatMapImagePath { get; set; }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NewEditAircraftConfigurationWindow window = this.window as NewEditAircraftConfigurationWindow;
            window.Top = 0;
            window.Left = SystemParameters.PrimaryScreenWidth / 2 - window.ActualWidth / 2;
            seatMapImage.Height = SystemParameters.FullPrimaryScreenHeight - 110; // 110 is the height of component in window minus by seatMapImage.Height
            seatMapImage.Width = seatMapImage.Height * 9 / 16; // 16:9 is an aspect ratio of widescreen
            imgCanvas.Height = seatMapImage.Height;
            imgCanvas.Width = seatMapImage.Width;

            if (window.mainFrame.CanGoBack)
            {
                backBtn.Visibility = Visibility.Visible;
                backBtn.IsEnabled = true;
            }
            if(AircraftConfiguration.SeatMapImagePath != null)
            {
                seatMapImage.Source = new BitmapImage(new Uri(AircraftConfiguration.SeatMapImagePath));
            }
            else
            {
                DataRow seatMapImageData = await dbCon.GetDataRow("AircraftConfiguration", new DataRow("AircraftConfigurationId", AircraftConfiguration.AircraftConfigurationId));
                if (seatMapImageData.HasData && seatMapImageData.Error == ERROR.NoError)
                    if (seatMapImageData.Get("SeatMapImage") != DBNull.Value)
                        seatMapImage.Source = seatMapImageData.Get("SeatMapImage").BlobToSource();
            }

            aisleXPosUpDown.Maximum = seatMapImage.Width;
            DrawSeatLine();
        }

        private async void DrawSeatLine()
        {
            if(Status == STATUS.NEW || AircraftConfiguration.SeatMapImagePath != null)
            {
                DataRow airlineRef = await dbCon.GetDataRow("AirlineReference", new DataRow("AirlineCode", Application.Current.Resources["AirlineCode"]));
                SolidColorBrush lineBrush = null, seatBrush = null;
                if (airlineRef.HasData && airlineRef.Error == ERROR.NoError)
                {
                    lineBrush = new SolidColorBrush(int.Parse(airlineRef.Get("LineColor").ToString()).GetColor());
                    seatBrush = new SolidColorBrush(int.Parse(airlineRef.Get("SeatColor").ToString()).GetColor());
                }

                DrawAisleXLine(lineBrush, seatMapImage.Width / 2, 10, Visibility.Hidden);

                PointCollection frontDoorPointCollection = new PointCollection();
                frontDoorPointCollection.Add(new Point(seatMapImage.Width / 4, seatMapImage.Height / 4 - 10));
                frontDoorPointCollection.Add(new Point(seatMapImage.Width / 4, seatMapImage.Height / 4 + 10));
                frontDoorPointCollection.Add(new Point(seatMapImage.Width / 4 + 20, seatMapImage.Height / 4));

                DrawDoor("frontDoor", frontDoorPointCollection, seatBrush, Visibility.Hidden);

                PointCollection rearDoorPointCollection = new PointCollection();
                rearDoorPointCollection.Add(new Point(seatMapImage.Width / 4, seatMapImage.Height - seatMapImage.Height / 4 - 10));
                rearDoorPointCollection.Add(new Point(seatMapImage.Width / 4, seatMapImage.Height - seatMapImage.Height / 4 + 10));
                rearDoorPointCollection.Add(new Point(seatMapImage.Width / 4 + 20, seatMapImage.Height - seatMapImage.Height / 4));

                DrawDoor("rearDoor", rearDoorPointCollection, seatBrush, Visibility.Hidden);

                DrawDefaultSeat("seat1A", seatWidthUpDown.Value != null ? (double)seatWidthUpDown.Value : 10,
                    seatHeightUpDown.Value != null ? (double)seatHeightUpDown.Value : 10,
                    seat1AYPosUpDown.Value != null ? (double)seat1AYPosUpDown.Value : seatMapImage.Width / 2 - 100,
                    seat1AXPosUpDown.Value != null ? (double)seat1AXPosUpDown.Value : seatMapImage.Height / 2 - 100,
                    seatBrush, Visibility.Hidden);

                DrawDefaultSeat("seat2A", seatWidthUpDown.Value != null ? (double)seatWidthUpDown.Value : 10,
                    seatHeightUpDown.Value != null ? (double)seatHeightUpDown.Value : 10,
                    seat1AYPosUpDown.Value != null ? (double)seat1AYPosUpDown.Value : seatMapImage.Width / 2 - 100,
                    seat1AXPosUpDown.Value != null ? (double)seat1AXPosUpDown.Value : seatMapImage.Height / 2 - 100,
                    seatBrush, Visibility.Hidden);

                for (int i = 1; i <= 5; i++)
                    AddItemToLocateComboBox(i, 0, numOfRowsUpDown.Value != null ? (int)numOfRowsUpDown.Value : 0, new List<int>().ToArray());
            } else
            {
                try
                {
                    int numOfRow = 0, numOfLeftColumn = 0, numOfRightColumn = 0;
                    double aisleXWidth = 0, aisleXPos = 0, frontDoorX = 0, frontDoorY = 0, frontDoorWidth = 0, rearDoorX = 0, rearDoorY = 0, rearDoorWidth = 0;
                    DataList seatMapList = await dbCon.GetDataList("SeatMap", new DataRow("AircraftConfigurationId", AircraftConfiguration.AircraftConfigurationId), "ORDER BY SeatRow, SeatColumn");
                    foreach (DataRow seatMap in seatMapList)
                    {
                        int row = (int)seatMap.Get("SeatRow");
                        if((double)seatMap.Get("PositionX") < AircraftConfiguration.AisleX)
                            numOfLeftColumn++;
                        else
                            numOfRightColumn++;

                        if (row > numOfRow) numOfRow = row;
                    }
                    numOfLeftColumn = numOfLeftColumn / (numOfRow >= 13 ? numOfRow -1 : numOfRow);
                    numOfRightColumn = numOfRightColumn / (numOfRow >= 13 ? numOfRow - 1 : numOfRow);
                    seatPosition = new Seat[numOfLeftColumn + numOfRightColumn, numOfRow >= 13 ? numOfRow - 1 : numOfRow];
                    foreach(DataRow seatMap in seatMapList)
                    {
                        int row = (int)seatMap.Get("SeatRow");
                        row = row >= 13 ? row - 1 : row;
                        int column = Char2Number(seatMap.Get("SeatColumn").ToString().ToCharArray().First());
                        seatPosition[column - 1, row - 1] = new Seat
                        {
                            X = toWidthRatio((double)seatMap.Get("PositionX")),
                            Y = toHeightRatio((double)seatMap.Get("PositionY")),
                            Width = toWidthRatio((double)seatMap.Get("SeatWidth")),
                            Height = toHeightRatio((double)seatMap.Get("SeatHeight"))
                        };
                    }

                    aisleXWidth = toWidthRatio((double)seatMapList.GetListAt(numOfLeftColumn).Get("PositionX")) 
                        - (toWidthRatio((double)seatMapList.GetListAt(numOfLeftColumn - 1).Get("PositionX")) 
                        + toWidthRatio((double)seatMapList.GetListAt(numOfLeftColumn - 1).Get("SeatWidth")));
                    aisleXPos = toWidthRatio(AircraftConfiguration.AisleX);

                    frontDoorX = toWidthRatio(AircraftConfiguration.FrontDoorX);
                    frontDoorY = toHeightRatio(AircraftConfiguration.FrontDoorY);
                    frontDoorWidth = toHeightRatio(AircraftConfiguration.FrontDoorWidth);

                    rearDoorX = toWidthRatio(AircraftConfiguration.RearDoorX);
                    rearDoorY = toHeightRatio(AircraftConfiguration.RearDoorY);
                    rearDoorWidth = toHeightRatio(AircraftConfiguration.RearDoorWidth);

                    numOfRowsUpDown.Value = int.Parse((await dbCon.GetDataRow(string.Format("SELECT MAX(SeatRow) From SeatMap Where AircraftConfigurationId = '{0}'",
                        AircraftConfiguration.AircraftConfigurationId))).Get("").ToString());
                    numOfColLeftUpDown.Value = numOfLeftColumn;
                    numOfColRightUpDown.Value = numOfRightColumn;

                    aisleXPosUpDown.Value = aisleXPos;
                    aisleXWidthUpDown.Value = aisleXWidth;

                    frontDoorXPosUpDown.Value = frontDoorX;
                    frontDoorYPosUpDown.Value = frontDoorY;
                    frontDoorWidthUpDown.Value = frontDoorWidth;

                    rearDoorXPosUpDown.Value = rearDoorX;
                    rearDoorYPosUpDown.Value = rearDoorY;
                    rearDoorWidthUpDown.Value = rearDoorWidth;

                    seat1AXPosUpDown.Value = toWidthRatio((double)seatMapList.GetListAt(0).Get("PositionX"));
                    seat1AYPosUpDown.Value = toHeightRatio((double)seatMapList.GetListAt(0).Get("PositionY"));

                    seat2AXPosUpDown.Value = toWidthRatio((double)seatMapList.GetListAt(numOfLeftColumn + numOfRightColumn).Get("PositionX"));
                    seat2AYPosUpDown.Value = toHeightRatio((double)seatMapList.GetListAt(numOfLeftColumn + numOfRightColumn).Get("PositionY"));

                    seatWidthUpDown.Value = toWidthRatio((double)seatMapList.GetListAt(0).Get("SeatWidth"));
                    seatHeightUpDown.Value = toHeightRatio((double)seatMapList.GetListAt(0).Get("SeatHeight"));

                    if (AircraftConfiguration.MiddleRow > -1) middleRowUpDown.Value = AircraftConfiguration.MiddleRow;

                    DataRow airlineRef = await dbCon.GetDataRow("AirlineReference", new DataRow("AirlineCode", Application.Current.Resources["AirlineCode"]));
                    SolidColorBrush lineBrush = null, seatBrush = null;
                    if (airlineRef.HasData && airlineRef.Error == ERROR.NoError)
                    {
                        lineBrush = new SolidColorBrush(int.Parse(airlineRef.Get("LineColor").ToString()).GetColor());
                        seatBrush = new SolidColorBrush(int.Parse(airlineRef.Get("SeatColor").ToString()).GetColor());
                    }

                    DrawAisleXLine(lineBrush, aisleXPos, aisleXWidth);

                    PointCollection frontDoorPointCollection = new PointCollection();
                    frontDoorPointCollection.Add(new Point(frontDoorX, frontDoorY - frontDoorWidth / 2));
                    frontDoorPointCollection.Add(new Point(frontDoorX, frontDoorY + frontDoorWidth / 2));
                    frontDoorPointCollection.Add(new Point(frontDoorX + frontDoorWidth, frontDoorY));

                    DrawDoor("frontDoor", frontDoorPointCollection, seatBrush);

                    PointCollection rearDoorPointCollection = new PointCollection();
                    rearDoorPointCollection.Add(new Point(rearDoorX, rearDoorY - rearDoorWidth / 2));
                    rearDoorPointCollection.Add(new Point(rearDoorX, rearDoorY + rearDoorWidth / 2));
                    rearDoorPointCollection.Add(new Point(rearDoorX + rearDoorWidth, rearDoorY));

                    DrawDoor("rearDoor", rearDoorPointCollection, seatBrush);
                    
                    if (airlineRef.HasData && airlineRef.Error == ERROR.NoError)
                        seatBrush = new SolidColorBrush(int.Parse(airlineRef.Get("SeatColor").ToString()).GetColor());
                    for (int i = 0; i < numOfLeftColumn + numOfRightColumn; i++)
                    for (int j = 0; j < (numOfRow >= 13 ? numOfRow - 1 : numOfRow); j++)
                    {
                        Grid seat = new Grid();
                        seat.Background = seatBrush != null ? seatBrush : Brushes.Red;
                        seat.Height = seatPosition[i, j].Height;
                        seat.Width = seatPosition[i, j].Width;

                        TextBlock text = new TextBlock();
                        text.Text = j + (j >= 12 ? 2 : 1) + Number2String(i + 1, true);
                        text.Foreground = Brushes.White;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.HorizontalAlignment = HorizontalAlignment.Center;
                        text.FontSize = 4;
                        seat.Children.Add(text);

                        seat.Name = "Seat" + text.Text;
                        RegisterName(seat.Name, seat);

                        imgCanvas.Children.Add(seat);
                        Canvas.SetTop(seat, seatPosition[i, j].Y);
                        Canvas.SetLeft(seat, seatPosition[i, j].X);
                    }

                    DrawDefaultSeat("seat1A", seatPosition[0, 0].Width, seatPosition[0, 0].Height,
                        seatPosition[0, 0].Y, seatPosition[0, 0].X, seatBrush, Visibility.Hidden);

                    DrawDefaultSeat("seat2A", seatPosition[0, 1].Width, seatPosition[0, 1].Height,
                        seatPosition[0, 1].Y, seatPosition[0, 1].X, seatBrush, Visibility.Hidden);

                    for (int i = 1; i <= 5; i++)
                        AddItemToLocateComboBox(i, 0, numOfRowsUpDown.Value != null ? (int)numOfRowsUpDown.Value : 0, new List<int>().ToArray());

                    editBtn.Visibility = Visibility.Visible;
                    nextBtn.Visibility = Visibility.Collapsed;
                    finishBtn.Visibility = Visibility.Visible;

                    EnableAllChildren(detailGrid, false, new List<Type> { typeof(DoubleUpDown), typeof(IntegerUpDown), typeof(ComboBox) }.ToArray());
                }
                catch (Exception) { }
            }
        }

        private void DrawDefaultSeat(string seatName, double width, double height, double top, double left,
            SolidColorBrush seatBrush, Visibility visibility = Visibility.Visible)
        {
            Rectangle seat = new Rectangle();
            seat.Fill = seatBrush != null ? seatBrush : Brushes.Red;
            seat.Height = width;
            seat.Width = height;
            seat.Name = seatName;
            seat.Visibility = visibility;
            RegisterName(seatName, seat);
            imgCanvas.Children.Add(seat);
            Canvas.SetTop(seat, top);
            Canvas.SetLeft(seat, left);
        }

        private void DrawDoor(string doorName, PointCollection frontDoorPointCollection, 
            SolidColorBrush seatBrush, Visibility visibility = Visibility.Visible)
        {
            Polygon door = new Polygon();
            door.Points = frontDoorPointCollection;
            door.Fill = seatBrush != null ? seatBrush : Brushes.Red;
            door.Stroke = Brushes.Black;
            door.StrokeThickness = 1;
            door.Name = doorName;
            door.Visibility = visibility;
            RegisterName(doorName, door);
            imgCanvas.Children.Add(door);
        }

        private void DrawAisleXLine(SolidColorBrush lineBrush, double aisleXPos, double aisleXWidth, Visibility visibility = Visibility.Visible)
        {
            Line aisleXLine = new Line
            {
                Stroke = lineBrush != null ? lineBrush : Brushes.Red,
                X1 = aisleXPos,
                X2 = aisleXPos,
                Y1 = 0,
                Y2 = seatMapImage.Height,
                StrokeThickness = aisleXWidth,
                Visibility = visibility
            };
            aisleXLine.Name = "aisleXLine";
            RegisterName("aisleXLine", aisleXLine);
            imgCanvas.Children.Add(aisleXLine);
        }

        private double toWidthRatio(double oldWidth)
        {
            return oldWidth * imgCanvas.Width / 1080;
        }

        private double toHeightRatio(double oldHeight)
        {
            return oldHeight * imgCanvas.Height / 1920;
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            GenerateSeatPosition();
        }

        private void AddItemToLocateComboBox(int indexOfAisleY, int selectedIndex, int numOfTotalRow, params int[] exceptsRows)
        {
            if(aisleYGroupBox != null)
            {
                ComboBox locateComboBox = aisleYGroupBox.FindName("locateY" + indexOfAisleY + "ComboBox") as ComboBox;
                if (locateComboBox != null)
                {
                    locateComboBox.Items.Clear();
                    for (int j = 1; j < numOfTotalRow; j++)
                        if(j != 13 && !exceptsRows.Contains(j))
                            locateComboBox.Items.Add(new CustomComboBoxItem { Text = j.ToString(), Value = j });
                    locateComboBox.SelectedIndex = selectedIndex > -1 ? selectedIndex : 0;
                }
            }
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            if(finishBtn.Visibility == Visibility.Visible && editBtn.Visibility == Visibility.Collapsed)
            {
                finishBtn.Visibility = Visibility.Collapsed;
                nextBtn.Visibility = Visibility.Visible;
                EnableAllChildren(detailGrid, true, new List<Type> { typeof(DoubleUpDown), typeof(IntegerUpDown), typeof(ComboBox) }.ToArray());
                ClearSeat();
                editingDetailTextBlock.Visibility = Visibility.Hidden;
                numOfRowsUpDown.Focus();
            } else
            {
                Frame mainFrame = (window as NewEditAircraftConfigurationWindow).mainFrame;
                if (mainFrame.CanGoBack)
                    mainFrame.GoBack();
            }
        }

        private void aisleXPosUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Line aisleXLine = imgCanvas.FindName("aisleXLine") as Line;
            if(aisleXLine != null)
            {
                aisleXLine.X1 = aisleXPosUpDown.Value != null ? (double)aisleXPosUpDown.Value : aisleXLine.X1;
                aisleXLine.X2 = aisleXPosUpDown.Value != null ? (double)aisleXPosUpDown.Value : aisleXLine.X2;
            }
            
        }

        private void aisleXWidthUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Line aisleXLine = imgCanvas.FindName("aisleXLine") as Line;
            if (aisleXLine != null)
                aisleXLine.StrokeThickness = aisleXWidthUpDown.Value != null ? (double)aisleXWidthUpDown.Value : aisleXLine.StrokeThickness;
        }

        private void AdjustAisleX(Visibility visibility)
        {
            editingDetailTextBlock.Text = "Adjust Aisle X Position and Width";
            editingDetailTextBlock.Visibility = visibility;
            Line aisleXLine = imgCanvas.FindName("aisleXLine") as Line;
            if (aisleXLine != null)
                aisleXLine.Visibility = visibility;
        }

        private void AisleAdjust_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearSeat();
            DoubleUpDown aisleXUpDown = sender as DoubleUpDown;
            if(aisleXUpDown.Value == null)
            {
                aisleXUpDown.ClearValue(Control.BorderBrushProperty);
                if (aisleXUpDown.Name == "aisleXPosUpDown")
                    aisleXUpDown.Value = seatMapImage.Width / 2;
                else
                    aisleXUpDown.Value = 10;
            }
            AdjustAisleX(Visibility.Visible);
        }

        private void AisleAdjust_LostFocus(object sender, RoutedEventArgs e)
        {
            AdjustAisleX(Visibility.Hidden);
        }

        private void numOfAisleYUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (numOfAisleYUpDown.Value != null)
            {
                int numOfAisleY = (int)numOfAisleYUpDown.Value;
                for (int i = 1; i <= 5; i++)
                    if (i <= numOfAisleY && numOfAisleY != 0)
                    {
                        StackPanel aisleYStack = aisleYGroupBox.FindName("aisleY" + i) as StackPanel;
                        if (aisleYStack != null)
                        {
                            aisleYStack.Visibility = Visibility.Visible;
                            DoubleUpDown aisleYWidthUpDown = aisleYStack.FindName(string.Format("aisleY{0}WidthUpDown", i)) as DoubleUpDown;
                            Line exitingLine = imgCanvas.FindName("exitLine" + i) as Line;
                            if (exitingLine == null)
                            {
                                Line exitLine = new Line
                                {
                                    Stroke = Brushes.Red,
                                    X1 = 0,
                                    X2 = seatMapImage.Width,
                                    Y1 = seatMapImage.Height / 2,
                                    Y2 = seatMapImage.Height / 2,
                                    StrokeThickness = aisleYWidthUpDown != null ? (double)aisleYWidthUpDown.Value : 10.0f,
                                    Visibility = Visibility.Hidden
                                };
                                exitLine.Name = "exitLine" + i;
                                RegisterName("exitLine" + i, exitLine);
                                imgCanvas.Children.Add(exitLine);
                                DoubleUpDown aisleYPosUpDown = aisleYStack.FindName(string.Format("aisleY{0}PosUpDown", i)) as DoubleUpDown;
                                if (aisleYPosUpDown != null)
                                {
                                    aisleYPosUpDown.Value = seatMapImage.Height / 2;
                                    aisleYPosUpDown.Maximum = seatMapImage.Height;
                                }
                            }
                        }
                    }
                    else
                    {
                        StackPanel aisleYStack = aisleYGroupBox.FindName("aisleY" + i) as StackPanel;
                        if (aisleYStack != null)
                        {
                            aisleYStack.Visibility = Visibility.Collapsed;
                            Line exitLine = imgCanvas.FindName("exitLine" + i) as Line;
                            if (exitLine != null)
                            {
                                UnregisterName(exitLine.Name);
                                imgCanvas.Children.Remove(exitLine);
                            }
                        }
                    }
            }
            else
            {
                numOfAisleYUpDown.Value = 0;
            }
        }

        private void numOfRowsUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (numOfAisleYUpDown != null && numOfRowsUpDown != null)
                {
                    numOfAisleYUpDown.Maximum = numOfRowsUpDown.Value - 1 > 5 ? 5 : numOfRowsUpDown.Value - 1;
                    for (int i = 1; i <= 5; i++)
                    {
                        ComboBox locateComboBox = aisleYGroupBox.FindName("locateY" + i + "ComboBox") as ComboBox;
                        int selectingIndex = 0;
                        if (locateComboBox != null)
                            selectingIndex = locateComboBox.SelectedIndex;
                        AddItemToLocateComboBox(i, selectingIndex, (int)numOfRowsUpDown.Value, new List<int>().ToArray());
                    }
                }
                if(numOfRowsUpDown.Value != null)
                {
                    middleRowUpDown.Maximum = numOfRowsUpDown.Value - 1;
                    middleRowUpDown.Value = (int)(numOfRowsUpDown.Value / 2);
                }
                
            }
            catch (Exception) { }   
        }

        private void ExitAdjust_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearSeat();
            decimal index = -1;
            decimal.TryParse(Regex.Match(sender is DoubleUpDown ? (sender as DoubleUpDown).Name : (sender as ComboBox).Name, @"\d+").Value, out index);
            ExitAdjust((int)index, Visibility.Visible);
        }

        private void ExitAdjust_LostFocus(object sender, RoutedEventArgs e)
        {
            decimal index = -1;
            decimal.TryParse(Regex.Match(sender is DoubleUpDown ? (sender as DoubleUpDown).Name : (sender as ComboBox).Name, @"\d+").Value, out index);
            ExitAdjust((int)index, Visibility.Hidden);
        }

        private void ExitAdjust(int index, Visibility visibility)
        {
            editingDetailTextBlock.Text = string.Format("Adjust Exit No.{0} Position and Width", index);
            editingDetailTextBlock.Visibility = visibility;
            Line exitLine = imgCanvas.FindName("exitLine" + index) as Line;
            if (exitLine != null)
                exitLine.Visibility = visibility;
        }

        private void exitPosChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DoubleUpDown exitPosUpDown = sender as DoubleUpDown;
            decimal index = -1;
            decimal.TryParse(Regex.Match(exitPosUpDown.Name, @"\d+").Value, out index);

            Line exitLine = imgCanvas.FindName("exitLine" + index) as Line;
            if (exitLine != null)
            {
                exitLine.Y1 = exitPosUpDown.Value != null ? (double)exitPosUpDown.Value : exitLine.Y1;
                exitLine.Y2 = exitPosUpDown.Value != null ? (double)exitPosUpDown.Value : exitLine.Y2;
            }
            
        }

        private void exitWidthChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DoubleUpDown exitWidthUpDown = sender as DoubleUpDown;
            decimal index = -1;
            decimal.TryParse(Regex.Match(exitWidthUpDown.Name, @"\d+").Value, out index);

            Line exitLine = imgCanvas.FindName("exitLine" + index) as Line;
            if (exitLine != null)
                exitLine.StrokeThickness = exitWidthUpDown.Value != null ? (double)exitWidthUpDown.Value : exitLine.StrokeThickness;
        }

        private void frontDoorXPosUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Polygon frontDoor = imgCanvas.FindName("frontDoor") as Polygon;
            if(frontDoor != null)
            {
                PointCollection newPoints = new PointCollection();
                newPoints.Add(new Point(frontDoorXPosUpDown.Value != null ? (double)frontDoorXPosUpDown.Value : frontDoor.Points[0].X,
                    frontDoor.Points[0].Y));
                newPoints.Add(new Point(frontDoorXPosUpDown.Value != null ? (double)frontDoorXPosUpDown.Value : frontDoor.Points[1].X,
                    frontDoor.Points[1].Y));
                newPoints.Add(new Point(frontDoorXPosUpDown.Value != null ? (double)frontDoorXPosUpDown.Value + 
                    (frontDoorWidthUpDown.Value != null ? (double)frontDoorWidthUpDown.Value : 20 ): frontDoor.Points[2].X,
                    frontDoor.Points[2].Y));
                frontDoor.Points = newPoints;
            }
            
        }

        private void frontDoorYPosUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Polygon frontDoor = imgCanvas.FindName("frontDoor") as Polygon;
            if (frontDoor != null)
            {
                PointCollection newPoints = new PointCollection();
                newPoints.Add(new Point(frontDoor.Points[0].X,
                    frontDoorYPosUpDown.Value != null ? (double)frontDoorYPosUpDown.Value - (frontDoorWidthUpDown.Value != null ? (double)frontDoorWidthUpDown.Value : 20f) / 2 : frontDoor.Points[0].Y));
                newPoints.Add(new Point(frontDoor.Points[1].X,
                    frontDoorYPosUpDown.Value != null ? (double)frontDoorYPosUpDown.Value + (frontDoorWidthUpDown.Value != null ? (double)frontDoorWidthUpDown.Value : 20f) / 2 : frontDoor.Points[0].Y));
                newPoints.Add(new Point(frontDoor.Points[2].X,
                    frontDoorYPosUpDown.Value != null ? (double)frontDoorYPosUpDown.Value : frontDoor.Points[2].Y));
                frontDoor.Points = newPoints;
            }
            
        }

        private void frontDoorWidthUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Polygon frontDoor = imgCanvas.FindName("frontDoor") as Polygon;
            if (frontDoor != null)
            {
                PointCollection newPoints = new PointCollection();
                newPoints.Add(new Point(frontDoorXPosUpDown.Value != null ? (double)frontDoorXPosUpDown.Value : frontDoor.Points[0].X,
                    frontDoorYPosUpDown.Value != null ? (double)frontDoorYPosUpDown.Value - (frontDoorWidthUpDown.Value != null ? (double)frontDoorWidthUpDown.Value : 20f) / 2 : frontDoor.Points[0].Y));
                newPoints.Add(new Point(frontDoorXPosUpDown.Value != null ? (double)frontDoorXPosUpDown.Value : frontDoor.Points[1].X,
                    frontDoorYPosUpDown.Value != null ? (double)frontDoorYPosUpDown.Value + (frontDoorWidthUpDown.Value != null ? (double)frontDoorWidthUpDown.Value : 20f) / 2 : frontDoor.Points[0].Y));
                newPoints.Add(new Point(frontDoorXPosUpDown.Value != null ? (double)frontDoorXPosUpDown.Value +
                    (frontDoorWidthUpDown.Value != null ? (double)frontDoorWidthUpDown.Value : 20) : frontDoor.Points[2].X,
                    frontDoorYPosUpDown.Value != null ? (double)frontDoorYPosUpDown.Value : frontDoor.Points[2].Y));
                frontDoor.Points = newPoints;
            }
            
        }

        private void frontDoor_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearSeat();
            DoubleUpDown frontDoorUpDown = sender as DoubleUpDown;
            if(frontDoorUpDown.Value == null)
            {
                frontDoorUpDown.ClearValue(Control.BorderBrushProperty);
                switch (frontDoorUpDown.Name)
                {
                    case "frontDoorXPosUpDown":
                        frontDoorUpDown.Value = seatMapImage.Width / 4;
                        break;
                    case "frontDoorYPosUpDown":
                        frontDoorUpDown.Value = seatMapImage.Height / 4;
                        break;
                    case "frontDoorWidthUpDown":
                        frontDoorUpDown.Value = 20f;
                        break;
                }
            }
            DoorFocus("frontDoor", Visibility.Visible);
        }

        private void frontDoor_LostFocus(object sender, RoutedEventArgs e)
        {
            DoorFocus("frontDoor", Visibility.Hidden);
        }

        private void rearDoorXPosUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Polygon rearDoor = imgCanvas.FindName("rearDoor") as Polygon;
            if (rearDoor != null)
            {
                PointCollection newPoints = new PointCollection();
                newPoints.Add(new Point(rearDoorXPosUpDown.Value != null ? (double)rearDoorXPosUpDown.Value : rearDoor.Points[0].X,
                    rearDoor.Points[0].Y));
                newPoints.Add(new Point(rearDoorXPosUpDown.Value != null ? (double)rearDoorXPosUpDown.Value : rearDoor.Points[1].X,
                    rearDoor.Points[1].Y));
                newPoints.Add(new Point(rearDoorXPosUpDown.Value != null ? (double)rearDoorXPosUpDown.Value +
                    (rearDoorWidthUpDown.Value != null ? (double)rearDoorWidthUpDown.Value : 20) : rearDoor.Points[2].X,
                    rearDoor.Points[2].Y));
                rearDoor.Points = newPoints;
            }
            
        }

        private void rearDoorYPosUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Polygon rearDoor = imgCanvas.FindName("rearDoor") as Polygon;
            if (rearDoor != null)
            {
                PointCollection newPoints = new PointCollection();
                newPoints.Add(new Point(rearDoor.Points[0].X,
                    rearDoorYPosUpDown.Value != null ? (double)rearDoorYPosUpDown.Value - (rearDoorWidthUpDown.Value != null ? (double)rearDoorWidthUpDown.Value : 20f) / 2 : rearDoor.Points[0].Y));
                newPoints.Add(new Point(rearDoor.Points[1].X,
                    rearDoorYPosUpDown.Value != null ? (double)rearDoorYPosUpDown.Value + (rearDoorWidthUpDown.Value != null ? (double)rearDoorWidthUpDown.Value : 20f) / 2 : rearDoor.Points[0].Y));
                newPoints.Add(new Point(rearDoor.Points[2].X,
                    rearDoorYPosUpDown.Value != null ? (double)rearDoorYPosUpDown.Value : rearDoor.Points[2].Y));
                rearDoor.Points = newPoints;
            }
            
        }

        private void rearDoorWidthUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Polygon rearDoor = imgCanvas.FindName("rearDoor") as Polygon;
            if (rearDoor != null)
            {
                PointCollection newPoints = new PointCollection();
                newPoints.Add(new Point(rearDoorXPosUpDown.Value != null ? (double)rearDoorXPosUpDown.Value : rearDoor.Points[0].X,
                    rearDoorYPosUpDown.Value != null ? (double)rearDoorYPosUpDown.Value - (rearDoorWidthUpDown.Value != null ? (double)rearDoorWidthUpDown.Value : 20f) / 2 : rearDoor.Points[0].Y));
                newPoints.Add(new Point(rearDoorXPosUpDown.Value != null ? (double)rearDoorXPosUpDown.Value : rearDoor.Points[1].X,
                    rearDoorYPosUpDown.Value != null ? (double)rearDoorYPosUpDown.Value + (rearDoorWidthUpDown.Value != null ? (double)rearDoorWidthUpDown.Value : 20f) / 2 : rearDoor.Points[0].Y));
                newPoints.Add(new Point(rearDoorXPosUpDown.Value != null ? (double)rearDoorXPosUpDown.Value +
                    (rearDoorWidthUpDown.Value != null ? (double)rearDoorWidthUpDown.Value : 20) : rearDoor.Points[2].X,
                    rearDoorYPosUpDown.Value != null ? (double)rearDoorYPosUpDown.Value : rearDoor.Points[2].Y));
                rearDoor.Points = newPoints;
            }
            
        }

        private void rearDoor_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearSeat();
            DoubleUpDown rearDoorUpDown = sender as DoubleUpDown;
            if(rearDoorUpDown.Value == null)
            {
                rearDoorUpDown.ClearValue(Control.BorderBrushProperty);
                switch (rearDoorUpDown.Name)
                {
                    case "rearDoorXPosUpDown":
                        rearDoorUpDown.Value = seatMapImage.Width / 4;
                        break;
                    case "rearDoorYPosUpDown":
                        rearDoorUpDown.Value = seatMapImage.Height - seatMapImage.Height / 4;
                        break;
                    case "rearDoorWidthUpDown":
                        rearDoorUpDown.Value = 20f;
                        break;
                }
            }
            DoorFocus("rearDoor", Visibility.Visible);
        }

        private void rearDoor_LostFocus(object sender, RoutedEventArgs e)
        {
            DoorFocus("rearDoor", Visibility.Hidden);
        }

        private void DoorFocus(string name, Visibility visibility)
        {
            editingDetailTextBlock.Text = string.Format("Adjust {0} Position and Width (Arrow's base must place at location of door)", name);
            editingDetailTextBlock.Visibility = visibility;
            Polygon door = imgCanvas.FindName(name) as Polygon;
            if(door != null)
                door.Visibility = visibility;
        }

        private void SeatAdjust(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string name = (sender as DoubleUpDown).Name;
            if (sender as DoubleUpDown == seat1AXPosUpDown || sender as DoubleUpDown == seat1AYPosUpDown)
            {
                Rectangle seat = imgCanvas.FindName("seat1A") as Rectangle;
                if (seat != null)
                {
                    double x = seat1AXPosUpDown.Value != null ? (double)seat1AXPosUpDown.Value : (double)seat.GetValue(Canvas.LeftProperty);
                    double y = seat1AYPosUpDown.Value != null ? (double)seat1AYPosUpDown.Value : (double)seat.GetValue(Canvas.TopProperty);

                    Canvas.SetLeft(seat, x);
                    Canvas.SetTop(seat, y);
                }
            } else if (sender as DoubleUpDown == seat2AXPosUpDown || sender as DoubleUpDown == seat2AYPosUpDown) {
                Rectangle seat = imgCanvas.FindName("seat2A") as Rectangle;
                if (seat != null)
                {
                    double x = seat2AXPosUpDown.Value != null ? (double)seat2AXPosUpDown.Value : (double)seat.GetValue(Canvas.LeftProperty);
                    double y = seat2AYPosUpDown.Value != null ? (double)seat2AYPosUpDown.Value : (double)seat.GetValue(Canvas.TopProperty);

                    Canvas.SetLeft(seat, x);
                    Canvas.SetTop(seat, y);
                }
            } else
            {
                Rectangle seat1A = imgCanvas.FindName("seat1A") as Rectangle;
                if (seat1A != null)
                {
                    double width = seatWidthUpDown.Value != null ? (double)seatWidthUpDown.Value : seat1A.Width;
                    double height = seatHeightUpDown.Value != null ? (double)seatHeightUpDown.Value : seat1A.Height;

                    seat1A.Width = width;
                    seat1A.Height = height;
                }
                Rectangle seat2A = imgCanvas.FindName("seat2A") as Rectangle;
                if (seat2A != null)
                {
                    double width = seatWidthUpDown.Value != null ? (double)seatWidthUpDown.Value : seat2A.Width;
                    double height = seatHeightUpDown.Value != null ? (double)seatHeightUpDown.Value : seat2A.Height;

                    seat2A.Width = width;
                    seat2A.Height = height;
                }
            }
        }

        private void SeatAdjust_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearSeat();
            DoubleUpDown seatUpDown = sender as DoubleUpDown;
            if(seatUpDown.Value == null)
            {
                seatUpDown.ClearValue(Control.BorderBrushProperty);
                switch (seatUpDown.Name)
                {
                    case "seat1AXPosUpDown":
                        seatUpDown.Value = seatMapImage.Width / 2 - 100;
                        break;
                    case "seat1AYPosUpDown":
                        seatUpDown.Value = seatMapImage.Height / 2 - 100;
                        break;
                    case "seat2AXPosUpDown":
                        seatUpDown.Value = seatMapImage.Width / 2 - 100;
                        break;
                    case "seat2AYPosUpDown":
                        seatUpDown.Value = seatMapImage.Height / 2 - 100;
                        break;
                    case "seatWidthUpDown":
                        seatUpDown.Value = 10;
                        break;
                    case "seatHeightUpDown":
                        seatUpDown.Value = 10;
                        break;
                }
            }
            if ((sender as DoubleUpDown).Name.Contains("1A")){
                SeatAdjust("1A", Visibility.Visible);
            } else if ((sender as DoubleUpDown).Name.Contains("2A")){
                SeatAdjust("2A", Visibility.Visible);
            }
            else
            {
                SeatAdjust("1A", Visibility.Visible);
                SeatAdjust("2A", Visibility.Visible);
            }
        }

        private void SeatAdjust_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as DoubleUpDown).Name.Contains("1A"))
            {
                SeatAdjust("1A", Visibility.Hidden);
            }
            else if ((sender as DoubleUpDown).Name.Contains("2A"))
            {
                SeatAdjust("2A", Visibility.Hidden);
            } else
            {
                SeatAdjust("1A", Visibility.Hidden);
                SeatAdjust("2A", Visibility.Hidden);
            }
        }

        private void SeatAdjust(string name, Visibility visibility)
        {
            editingDetailTextBlock.Text = string.Format("Adjust {0} Seat Position, Width and Height", name);
            editingDetailTextBlock.Visibility = visibility;
            name = "seat" + name;
            Rectangle seat = imgCanvas.FindName(name) as Rectangle;
            if (seat != null)
                seat.Visibility = visibility;
        }

        private async void GenerateSeatPosition()
        {
            if (!IsFulFill()) return;
            try
            {
                ClearSeat();

                int numOfRows = (int)numOfRowsUpDown.Value >= 13 ? (int)numOfRowsUpDown.Value - 1 : (int)numOfRowsUpDown.Value;
                int numOfLeftCol = (int)numOfColLeftUpDown.Value;
                int numOfRightCol = (int)numOfColRightUpDown.Value;
                int numOfAisleY = (int)numOfAisleYUpDown.Value;
                seatPosition = new Seat[numOfLeftCol + numOfRightCol, numOfRows];
                double seat1AXPos = (double)seat1AXPosUpDown.Value;
                double seat1AYPos = (double)seat1AYPosUpDown.Value;
                double seat2AXPos = (double)seat2AXPosUpDown.Value;
                double seat2AYPos = (double)seat2AYPosUpDown.Value;
                double seatWidth = (double)seatWidthUpDown.Value;
                double seatHeight = (double)seatHeightUpDown.Value;
                double aisleXPos = (double)aisleXPosUpDown.Value;
                double aisleXWidth = (double)aisleXWidthUpDown.Value;
                double marginBetweenRows = seat2AYPos - seat1AYPos - seatHeight;


                AisleY[] aisleYs = new AisleY[5];
                for (int i = 0; i < numOfAisleY; i++)
                    if((aisleYGroupBox.FindName(string.Format("locateY{0}ComboBox", i + 1)) as ComboBox).SelectedValue != null)
                        aisleYs[i] = new AisleY
                        {
                            Height = (double)(aisleYGroupBox.FindName(string.Format("aisleY{0}WidthUpDown", i + 1)) as DoubleUpDown).Value,
                            AfterRow = (int)(aisleYGroupBox.FindName(string.Format("locateY{0}ComboBox", i + 1)) as ComboBox).SelectedValue
                        };

                for (int i = 0; i < numOfLeftCol; i++) // column loop
                for (int j = 0; j < numOfRows; j++)
                {
                    double x = seat1AXPos + i * seatWidth;
                    double y = 0;
                    if (j == 0)
                    {
                        y = seat1AYPos;
                    }
                    else
                    {
                        y = seatPosition[i, j - 1].Y;
                        if(numOfAisleY == 0)
                        {
                            y += seatHeight + marginBetweenRows;
                        } else
                        {
                            bool added = false;
                            for(int k = 0; k < numOfAisleY; k++)
                                if(aisleYs[k] != null)
                                    if (j == (aisleYs[k].AfterRow >= 13 ? aisleYs[k].AfterRow - 1 : aisleYs[k].AfterRow))
                                    {
                                        added = true;
                                        y += seatHeight + aisleYs[k].Height;
                                    }
                            if (!added)
                                y += seatHeight + marginBetweenRows;
                        }
                    }
                    seatPosition[i, j] = new Seat
                    {
                        X = x,
                        Y = y
                    };
                }

                for (int i = 0; i < numOfRightCol; i++) // column loop
                for (int j = 0; j < numOfRows; j++)
                {
                    double x = seat1AXPos + aisleXWidth + (i + numOfLeftCol) * seatWidth;
                    double y = 0;
                    if (j == 0)
                    {
                        y = seat1AYPos;
                    }
                    else
                    {
                        y = seatPosition[i, j - 1].Y;
                        if (numOfAisleY == 0)
                        {
                            y += seatHeight + marginBetweenRows;
                        }
                        else
                        {
                            bool added = false;
                            for (int k = 0; k < numOfAisleY; k++)
                                if (aisleYs[k] != null)
                                    if (j == (aisleYs[k].AfterRow >= 13 ? aisleYs[k].AfterRow - 1 : aisleYs[k].AfterRow))
                                    {
                                        added = true;
                                        y += seatHeight + aisleYs[k].Height;
                                    }
                            if (!added)
                                y += seatHeight + marginBetweenRows;
                        }
                    }
                    seatPosition[i + numOfLeftCol, j] = new Seat
                    {
                        X = x,
                        Y = y
                    };
                }

                DataRow airlineRef = await dbCon.GetDataRow("AirlineReference", new DataRow("AirlineCode", Application.Current.Resources["AirlineCode"]));
                SolidColorBrush seatBrush = null;
                if (airlineRef.HasData && airlineRef.Error == ERROR.NoError)
                    seatBrush = new SolidColorBrush(int.Parse(airlineRef.Get("SeatColor").ToString()).GetColor());
                for (int i = 0; i < numOfLeftCol + numOfRightCol; i++)
                for (int j = 0; j < numOfRows; j++)
                {
                    Grid seat = new Grid();
                    seat.Background = seatBrush != null ? seatBrush : Brushes.Red;
                    seat.Height = seatHeightUpDown.Value != null ? (double)seatHeightUpDown.Value : 10;
                    seat.Width = seatWidthUpDown.Value != null ? (double)seatWidthUpDown.Value : 10;

                    TextBlock text = new TextBlock();
                    text.Text = j + (j >= 12 ? 2 : 1) + Number2String(i + 1, true);
                    text.Foreground = Brushes.White;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.FontSize = 4;
                    seat.Children.Add(text);

                    seat.Name = "Seat" + text.Text;
                    RegisterName(seat.Name, seat);

                    imgCanvas.Children.Add(seat);
                    Canvas.SetTop(seat, seatPosition[i, j].Y);
                    Canvas.SetLeft(seat, seatPosition[i, j].X);
                }

                foreach (UIElement elem in imgCanvas.Children)
                {
                    if(elem is Line)
                    {
                        Line line = elem as Line;
                        if (line.Name == "aisleXLine")
                            line.Visibility = Visibility.Visible;
                    }
                    if(elem is Polygon)
                    {
                        Polygon door = elem as Polygon;
                        if (door.Name == "frontDoor" || door.Name == "rearDoor")
                            door.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        private string Number2String(int number, bool isCaps)
        {
            char c = (char)((isCaps ? 65 : 97) + (number - 1));
            return c.ToString();
        }

        private int Char2Number(char c)
        {
            int index = char.ToUpper(c) - 64;
            return index;
        }

        private void numOfRowsUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearSeat();
            IntegerUpDown numOfRowUpDown = sender as IntegerUpDown;
            if(numOfRowUpDown.Value == null)
            {
                numOfRowUpDown.ClearValue(Control.BorderBrushProperty);
                numOfRowsUpDown.Value = 0;
            }
        }

        private void numOfColLeftUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearSeat();
            IntegerUpDown numOfColLeftUpDown = sender as IntegerUpDown;
            if(numOfColLeftUpDown.Value == null)
            {
                numOfColLeftUpDown.ClearValue(Control.BorderBrushProperty);
                numOfColLeftUpDown.Value = 0;
            }
        }

        private void numOfColRightUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearSeat();
            IntegerUpDown numOfColRightUpDown = sender as IntegerUpDown;
            if(numOfColRightUpDown.Value == null)
            {
                numOfColRightUpDown.ClearValue(Control.BorderBrushProperty);
                numOfColRightUpDown.Value = 0;
            }
        }

        private bool IsFulFill()
        {
            bool result = true;
            if (numOfRowsUpDown.Value == null)
            {
                numOfRowsUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if(middleRowUpDown.Value == null)
            {
                middleRowUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (numOfColLeftUpDown.Value == null)
            {
                numOfColLeftUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (numOfColRightUpDown.Value == null)
            {
                numOfColRightUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (numOfAisleYUpDown.Value == null)
            {
                numOfAisleYUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (seat1AXPosUpDown.Value == null)
            {
                seat1AXPosUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (seat1AYPosUpDown.Value == null)
            {
                seat1AYPosUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (seat2AXPosUpDown.Value == null)
            {
                seat2AXPosUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (seat2AYPosUpDown.Value == null)
            {
                seat2AYPosUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (seatWidthUpDown.Value == null)
            {
                seatWidthUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (seatHeightUpDown.Value == null)
            {
                seatHeightUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (aisleXPosUpDown.Value == null)
            {
                aisleXPosUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (aisleXWidthUpDown.Value == null)
            {
                aisleXWidthUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (frontDoorXPosUpDown.Value == null)
            {
                frontDoorXPosUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (frontDoorYPosUpDown.Value == null)
            {
                frontDoorYPosUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (frontDoorWidthUpDown.Value == null)
            {
                frontDoorWidthUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (rearDoorXPosUpDown.Value == null)
            {
                rearDoorXPosUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (rearDoorYPosUpDown.Value == null)
            {
                rearDoorYPosUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (rearDoorWidthUpDown.Value == null)
            {
                rearDoorWidthUpDown.BorderBrush = Brushes.Red;
                result = false;
            }
            if (!result)
            {
                editingDetailTextBlock.Text = "Please complete fills before generate seats";
                editingDetailTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                editingDetailTextBlock.Text = "Click Finish to save aircraft configuration or Click Back to edit";
                editingDetailTextBlock.Visibility = Visibility.Visible;
                List<Type> types = new List<Type>
                {
                    typeof(DoubleUpDown),
                    typeof(IntegerUpDown),
                    typeof(ComboBox)
                };
                EnableAllChildren(detailGrid, false, types.ToArray());
                nextBtn.Visibility = Visibility.Collapsed;
                finishBtn.Visibility = Visibility.Visible;
            }
            return result;
        }

        public static void EnableAllChildren(Visual myVisual, bool enableFlag = true, params Type[] types)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                // Retrieve child visual at specified index value.
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);

                // Do processing of the child visual object.
                foreach(Type t in types)
                    if(childVisual.GetType() == t)
                    {
                        PropertyInfo isEnabled = childVisual.GetType().GetProperty("IsEnabled");
                        isEnabled.SetValue(childVisual, enableFlag, null);
                    }

                // Enumerate children of the child visual object.
                EnableAllChildren(childVisual, enableFlag, types);
            }
        }

        private async void finishBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double canvasWidth = imgCanvas.Width;
                double canvasHeight = imgCanvas.Height;

                double aisleX = (double)aisleXPosUpDown.Value;
                double frontDoorX = (double)frontDoorXPosUpDown.Value;
                double frontDoorY = (double)frontDoorYPosUpDown.Value;
                double rearDoorX = (double)rearDoorXPosUpDown.Value;
                double rearDoorY = (double)rearDoorYPosUpDown.Value;

                int numOfRow = (int)numOfRowsUpDown.Value >= 13 ? (int)numOfRowsUpDown.Value - 1 : (int)numOfRowsUpDown.Value;
                int numOfColumn = (int)numOfColLeftUpDown.Value + (int)numOfColRightUpDown.Value;

                // assume most screen default resolution is 1920x1080
                aisleX = toDefaultWidth(aisleX);
                frontDoorX = toDefaultWidth(frontDoorX);
                frontDoorY = toDefaultHeight(frontDoorY);
                rearDoorX = toDefaultWidth(rearDoorX);
                rearDoorY = toDefaultHeight(rearDoorY);

                AircraftConfiguration.AisleX = aisleX;
                AircraftConfiguration.FrontDoorX = frontDoorX;
                AircraftConfiguration.FrontDoorY = frontDoorY;
                AircraftConfiguration.RearDoorX = rearDoorX;
                AircraftConfiguration.RearDoorY = rearDoorY;
                AircraftConfiguration.FrontDoorWidth = toDefaultHeight((double)frontDoorWidthUpDown.Value);
                AircraftConfiguration.RearDoorWidth = toDefaultHeight((double)rearDoorWidthUpDown.Value);
                AircraftConfiguration.MiddleRow = (int)middleRowUpDown.Value;

                DataRow aircraftConfiguration = new DataRow();
                PropertyInfo[] acProps = AircraftConfiguration.GetType().GetProperties();
                foreach (PropertyInfo property in acProps)
                    if (property.Name.Equals("AircraftType"))
                        aircraftConfiguration.Set("AircraftTypeCode", AircraftConfiguration.AircraftType.AircraftTypeCode);
                    else if(!property.Name.Equals("SeatMapImagePath") && !property.Name.Equals("AircraftConfigurationId") && !property.Name.Equals("Status"))
                        aircraftConfiguration.Set(property.Name, property.GetValue(AircraftConfiguration, null));
                aircraftConfiguration.Set("CommitBy", Application.Current.Resources["UserAccountId"]);
                aircraftConfiguration.Set("CommitDateTime", DateTime.Now);
                
                if(Status == STATUS.NEW)
                {
                    DataRow result = await dbCon.CreateNewRowAndGetUId("AircraftConfiguration", aircraftConfiguration, "AircraftConfigurationId");
                    if (result.HasData && result.Error == ERROR.NoError)
                    {
                        Guid acId = (Guid)result.Get("AircraftConfigurationId");
                        await dbCon.UpdateBlobData("AircraftConfiguration", "SeatMapImage", AircraftConfiguration.SeatMapImagePath, new DataRow("AircraftConfigurationId", acId));
                        for (int column = 1; column <= numOfColumn; column++)
                        for (int row = 1; row <= numOfRow; row++)
                        {
                            DataRow seat = new DataRow(
                                "AircraftConfigurationId", acId,
                                "SeatRow", row >= 13 ? row + 1 : row,
                                "SeatColumn", Number2String(column, true),
                                "PositionX", toDefaultWidth(seatPosition[column - 1, row - 1].X),
                                "PositionY", toDefaultHeight(seatPosition[column - 1, row - 1].Y),
                                "SeatWidth", toDefaultWidth((double)seatWidthUpDown.Value),
                                "SeatHeight", toDefaultHeight((double)seatHeightUpDown.Value),
                                "CommitBy", Application.Current.Resources["UserAccountId"],
                                "CommitDateTime", DateTime.Now
                            );
                            if (!await dbCon.CreateNewRow("SeatMap", seat, "SeatMapId"))
                                throw new Exception();
                        }
                        window.DialogResult = true;
                        window.Hide();
                        System.Windows.MessageBox.Show(Messages.SUCCESS_ADD_AIRCRAFT_CONFIG, Captions.SUCCESS);
                        window.Close();
                    }
                    else
                    {
                        throw new Exception();
                    }
                } else
                {
                    bool result = await dbCon.UpdateDataRow("AircraftConfiguration", aircraftConfiguration, new DataRow("AircraftConfigurationId", AircraftConfiguration.AircraftConfigurationId));
                    if (result)
                    {
                        Guid acId = AircraftConfiguration.AircraftConfigurationId;
                        if(AircraftConfiguration.SeatMapImagePath != null)
                            await dbCon.UpdateBlobData("AircraftConfiguration", "SeatMapImage", AircraftConfiguration.SeatMapImagePath, new DataRow("AircraftConfigurationId", acId));
                        await dbCon.CustomQuery(SQLStatementType.DELETE, string.Format("DELETE FROM {0} WHERE {1} = '{2}'", "SeatMap", "AircraftConfigurationId", acId));
                        for (int column = 1; column <= numOfColumn; column++)
                        for (int row = 1; row <= numOfRow; row++)
                        {
                            DataRow seat = new DataRow(
                                "AircraftConfigurationId", acId,
                                "SeatRow", row >= 13 ? row + 1 : row,
                                "SeatColumn", Number2String(column, true),
                                "PositionX", toDefaultWidth(seatPosition[column - 1, row - 1].X),
                                "PositionY", toDefaultHeight(seatPosition[column - 1, row - 1].Y),
                                "SeatWidth", toDefaultWidth((double)seatWidthUpDown.Value),
                                "SeatHeight", toDefaultHeight((double)seatHeightUpDown.Value),
                                "CommitBy", Application.Current.Resources["UserAccountId"],
                                "CommitDateTime", DateTime.Now
                            );
                            if (!await dbCon.CreateNewRow("SeatMap", seat, "SeatMapId"))
                                throw new Exception();
                        }
                        window.DialogResult = true;
                        window.Hide();
                        System.Windows.MessageBox.Show(Messages.SUCCESS_UPDATE_AIRCRAFT_CONFIG, Captions.SUCCESS);
                        window.Close();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception)
            {
                if (Status == STATUS.NEW)
                    System.Windows.MessageBox.Show(Messages.ERROR_ADD_AIRCRAFT_CONFIG, Captions.ERROR);
                else
                    System.Windows.MessageBox.Show(Messages.ERROR_UPDATE_AIRCRAFT_CONFIG, Captions.ERROR);;
                window.DialogResult = false;
                window.Close();
            }
        }

        private double toDefaultWidth(double width)
        {
            return width / imgCanvas.Width * 1080;
        }

        private double toDefaultHeight(double height)
        {
            return height / imgCanvas.Height * 1920;
        }

        private void ClearSeat()
        {
            List<Grid> toRemove = new List<Grid>();
            foreach (UIElement elem in imgCanvas.Children)
                if (elem is Grid)
                {
                    Grid el = elem as Grid;
                    if (el.Name.Substring(0, 4) == "Seat")
                        toRemove.Add(el);
                }
            foreach (Grid elem in toRemove)
            {
                imgCanvas.Children.Remove(elem);
                UnregisterName(elem.Name);
            }
            foreach (UIElement elem in imgCanvas.Children)
            {
                if (elem is Line)
                {
                    Line line = elem as Line;
                    if (line.Name == "aisleXLine")
                        line.Visibility = Visibility.Hidden;
                }
                if (elem is Polygon)
                {
                    Polygon door = elem as Polygon;
                    if (door.Name == "frontDoor" || door.Name == "rearDoor")
                        door.Visibility = Visibility.Hidden;
                }
            }
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            EnableAllChildren(detailGrid, true, new List<Type> { typeof(DoubleUpDown), typeof(IntegerUpDown), typeof(ComboBox) }.ToArray());
            (sender as Button).Visibility = Visibility.Collapsed;
            nextBtn.Visibility = Visibility.Visible;
            finishBtn.Visibility = Visibility.Collapsed;
        }

        private void middleRowUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearSeat();
            IntegerUpDown middleRowUpDown = sender as IntegerUpDown;
            if (middleRowUpDown.Value == null)
            {
                middleRowUpDown.ClearValue(Control.BorderBrushProperty);
                middleRowUpDown.Value = 0;
            }
        }
    }
}
