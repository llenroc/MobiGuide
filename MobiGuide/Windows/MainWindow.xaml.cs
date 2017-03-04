using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
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
using System.ComponentModel;
using DatabaseConnector;
using CustomExtensions;
using System.Diagnostics;
using Properties;
using MobiGuide.Helper;
using Xceed.Wpf.Toolkit;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ResourceDictionary res = Application.Current.Resources;
        DBConnector dbCon = new DBConnector();
        public MainWindow()
        {
            InitializeComponent();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                displayInfo();
                InitializeControl();
            } catch (Exception){
                System.Windows.MessageBox.Show("Unexpected Error Occurred! Please contact Administator.", "Error");
                this.Close();
            }
        }

        private async void displayInfo()
        {
            //Get Full Name of current user
            DataRow userCondi = new DataRow();
            userCondi.Set("UserAccountId", res["UserAccountId"]);
            nameTxtBlock.Text = await dbCon.GetFullNameFromUid(res["UserAccountId"].ToString());

            DataRow airlineRefCondi = new DataRow();
            airlineRefCondi.Set("AirlineCode", res["AirlineCode"]);
            DataRow airlineRef = await dbCon.GetDataRow("AirlineReference", airlineRefCondi);
            airlineNameTxtBlock.Text = airlineRef.Get("AirlineName").ToString();

            ImageSource logoSource = ((object)airlineRef.Get("AirlineLogoSmall")).BlobToSource();
            if (logoSource != null) logoImg.Source = logoSource;
        }
        private void AddNewUserMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addUserWindow = new AddUserWindow();
            addUserWindow.ShowDialog();
        }

        private void EditCurrentUserMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditProfileWindow editProfileWindow = new EditProfileWindow();
            editProfileWindow.ShowDialog();
            if (editProfileWindow.DialogResult.HasValue && editProfileWindow.DialogResult.Value)
            {
                displayInfo();
                InitializeControl();
            }
        }

        private void LogoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Do you want to logout?", "Confirm", System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
            {
                //clear everything and do logout
                Application.Current.Resources.Clear();
                nameTxtBlock.Text = String.Empty;
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void EditAirlineReferenceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditAirlineReferenceWindow editALRefWindow = new EditAirlineReferenceWindow();
            editALRefWindow.ShowDialog();
            if (editALRefWindow.DialogResult.HasValue)
            {
                displayInfo();
                InitializeControl();
            }
        }

        private void AddAirportReferenceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddAirportReferenceWindow addAirportReferenceWindow = new AddAirportReferenceWindow();
            addAirportReferenceWindow.ShowDialog();
            if(addAirportReferenceWindow.DialogResult.HasValue)
            {
                displayInfo();
                InitializeControl();
            }
        }

        private void EditAirportReferenceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditAirportReferenceWindow editAirportReferenceWindow = new EditAirportReferenceWindow();
            editAirportReferenceWindow.ShowDialog();
            if(editAirportReferenceWindow.DialogResult.HasValue)
            {
                displayInfo();
                InitializeControl();
            }
        }

        private void AddLanguageReference_Click(object sender, RoutedEventArgs e)
        {
            AddLanguageReferenceWindow addLanguageReferenceWindow = new AddLanguageReferenceWindow();
            addLanguageReferenceWindow.ShowDialog();
            if(addLanguageReferenceWindow.DialogResult.HasValue)
            {
                displayInfo();
                InitializeControl();
            }
        }

        private void EditLanguageReference_Click(object sender, RoutedEventArgs e)
        {
            EditLanguageReferenceWindow editLanguageReferenceWindow = new EditLanguageReferenceWindow();
            editLanguageReferenceWindow.ShowDialog();
            if(editLanguageReferenceWindow.DialogResult.HasValue)
            {
                displayInfo();
                InitializeControl();
            }
        }

        private void AddAirportTranslationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddAirportTranslationWindow addAirportTranslationWindow = new AddAirportTranslationWindow();
            addAirportTranslationWindow.ShowDialog();
            if(addAirportTranslationWindow.DialogResult.HasValue)
            {
                displayInfo();
                InitializeControl();
            }
        }

        private void EditAirportTranslationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditAirportTranslationWindow editAirportTranslationWindow = new EditAirportTranslationWindow();
            editAirportTranslationWindow.ShowDialog();
            if(editAirportTranslationWindow.DialogResult.HasValue)
            {
                displayInfo();
                InitializeControl();
            }
        }
        private void AirportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AirportReferenceWindow airportReferenceWindow = new AirportReferenceWindow();
            airportReferenceWindow.ShowDialog();
            if(airportReferenceWindow.DialogResult.HasValue)
            {
                displayInfo();
                InitializeControl();
            }
        }

        private void TextTemplate_Click(object sender, RoutedEventArgs e)
        {
            TextTemplateWindow textTemplateWindow = new TextTemplateWindow();
            textTemplateWindow.ShowDialog();
            if(textTemplateWindow.DialogResult.HasValue)
            {
                displayInfo();
                InitializeControl();
            }
        }

        private void AircraftType_Click(object sender, RoutedEventArgs e)
        {
            AircraftTypeWindow aircraftTypeWindow = new AircraftTypeWindow();
            aircraftTypeWindow.ShowDialog();
            if(aircraftTypeWindow.DialogResult.HasValue)
            {
                displayInfo();
                InitializeControl();
            }
        }

        private void AircraftConfiguration_Click(object sender, RoutedEventArgs e)
        {
            AircraftConfigurationWindow aircraftConfigurationWindow = new AircraftConfigurationWindow();
            aircraftConfigurationWindow.ShowDialog();
            if(aircraftConfigurationWindow.DialogResult.HasValue)
            {
                displayInfo();
                InitializeControl();
            }
        }

        private async void InitializeControl()
        {
            DataList aircraftConfigurationDatas = await dbCon.GetDataList("AircraftConfiguration", new DataRow("StatusCode", "A"), "ORDER BY AircraftConfigurationCode");
            string selectedAircraftConfig = aircraftConfigComboBox.SelectedValue != null ? aircraftConfigComboBox.SelectedValue.ToString() : String.Empty;
            aircraftConfigComboBox.Items.Clear();
            if (aircraftConfigurationDatas.HasData && aircraftConfigurationDatas.Error == ERROR.NoError)
            {
                foreach(DataRow aircraftConfigData in aircraftConfigurationDatas)
                {
                    aircraftConfigComboBox.Items.Add(new CustomComboBoxItem
                    {
                        Text = aircraftConfigData.Get("AircraftConfigurationCode").ToString(),
                        Value = aircraftConfigData.Get("AircraftConfigurationId").ToString()
                    });
                }

                if (selectedAircraftConfig != String.Empty && aircraftConfigComboBox.Items.Cast<CustomComboBoxItem>().Any(item => item.Value.Equals(selectedAircraftConfig)))
                    aircraftConfigComboBox.SelectedValue = selectedAircraftConfig;
                else
                    aircraftConfigComboBox.SelectedIndex = 0;
            } else
            {
                aircraftConfigComboBox.Items.Add(new CustomComboBoxItem
                {
                    Text = "No Data",
                    Value = String.Empty
                });
                aircraftConfigComboBox.SelectedIndex = 0;
            }
             
            DataList airportReferenceDatas = await dbCon.GetDataList("AirportReference", new DataRow("StatusCode", "A"), "ORDER BY AirportName");
            string selectedOrigin = originComboBox.SelectedValue != null ? originComboBox.SelectedValue.ToString() : String.Empty;
            string selectedDestination = destinationComboBox.SelectedValue != null ? destinationComboBox.SelectedValue.ToString() : String.Empty;
            originComboBox.Items.Clear();
            destinationComboBox.Items.Clear();
            if(airportReferenceDatas.HasData && airportReferenceDatas.Error == ERROR.NoError)
            {
                foreach (DataRow airportReference in airportReferenceDatas)
                {
                    originComboBox.Items.Add(new CustomComboBoxItem
                    {
                        Text = airportReference.Get("AirportName").ToString(),
                        Value = airportReference.Get("AirportCode").ToString()
                    });

                    destinationComboBox.Items.Add(new CustomComboBoxItem
                    {
                        Text = airportReference.Get("AirportName").ToString(),
                        Value = airportReference.Get("AirportCode").ToString()
                    });
                }

                if (selectedOrigin != String.Empty && originComboBox.Items.Cast<CustomComboBoxItem>().Any(item => item.Value.Equals(selectedOrigin)))
                    originComboBox.SelectedValue = selectedOrigin;
                else
                    originComboBox.SelectedIndex = 0;

                if (selectedDestination != String.Empty && destinationComboBox.Items.Cast<CustomComboBoxItem>().Any(item => item.Value.Equals(selectedDestination)))
                    destinationComboBox.SelectedValue = selectedDestination;
                else
                    destinationComboBox.SelectedIndex = 0;
            } else
            {
                originComboBox.Items.Add(new CustomComboBoxItem
                {
                    Text = "No Data",
                    Value = String.Empty
                });

                destinationComboBox.Items.Add(new CustomComboBoxItem
                {
                    Text = "No Data",
                    Value = String.Empty
                });

                originComboBox.SelectedIndex = 0;
                destinationComboBox.SelectedIndex = 0;
            }

            DataList textTemplateDatas = await dbCon.GetDataList("TextTemplate", new DataRow("StatusCode", "A"), "ORDER BY TextName");
            string selectedTextTemplate = textTemplateComboBox.SelectedValue != null ? textTemplateComboBox.SelectedValue.ToString() : String.Empty;
            textTemplateComboBox.Items.Clear();
            if(textTemplateDatas.HasData && textTemplateDatas.Error == ERROR.NoError)
            {
                foreach(DataRow textTemplateData in textTemplateDatas)
                {
                    textTemplateComboBox.Items.Add(new CustomComboBoxItem
                    {
                        Text = textTemplateData.Get("TextName").ToString(),
                        Value = textTemplateData.Get("TextTemplateId").ToString()
                    });
                }
                if(selectedTextTemplate != String.Empty && textTemplateComboBox.Items.Cast<CustomComboBoxItem>().Any(item => item.Value.Equals(selectedTextTemplate)))
                    textTemplateComboBox.SelectedValue = selectedTextTemplate;
                else
                    textTemplateComboBox.SelectedIndex = 0;
            } else
            {
                textTemplateComboBox.Items.Add(new CustomComboBoxItem { Text = "No Data", Value = String.Empty });
                textTemplateComboBox.SelectedIndex = 0;
            }

            if(departureTimePicker.Value == null) departureTimePicker.Value = DateTime.Now;
        }

        private void showLogoBtn_Click(object sender, RoutedEventArgs e)
        {
            bool IsFulfill = true;
            if (aircraftConfigComboBox.SelectedValue == null || aircraftConfigComboBox.SelectedValue.Equals(String.Empty))
            {
                (aircraftConfigComboBox.Parent as Border).BorderBrush = Brushes.Red;
                aircraftConfigComboBox.GotFocus += AircraftConfigComboBox_GotFocus;
                IsFulfill = false;
            }
            if (frontDoorCheckBox.IsChecked == false && rearDoorCheckBox.IsChecked == false)
            {
                frontDoorCheckBox.BorderBrush = Brushes.Red;
                rearDoorCheckBox.BorderBrush = Brushes.Red;
                frontDoorCheckBox.GotFocus += FrontDoorCheckBox_GotFocus;
                rearDoorCheckBox.GotFocus += RearDoorCheckBox_GotFocus;
                IsFulfill = false;
            }
            if (!IsFulfill) return;
            Guid aircraftConfigId = new Guid(aircraftConfigComboBox.SelectedValue.ToString());
            ShowWindow(DISPLAY_TYPE.LOGO, null, aircraftConfigId, frontDoorCheckBox.IsChecked, rearDoorCheckBox.IsChecked);
        }

        private void ShowWindow(DISPLAY_TYPE type, params object[] param)
        {
            if (System.Windows.Forms.Screen.AllScreens.Count() > 1)
            {
                foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
                {
                    if (screen != System.Windows.Forms.Screen.AllScreens[0])
                    {
                        if (!EventHelper.IsWindowOpen<DisplayWindow>())
                        {
                            DisplayWindow displayWindow = new DisplayWindow(type, param);
                            displayWindow.Show();
                            displayWindow.MaximizeToSecondaryMonitor();
                        }
                        else
                        {
                            foreach (Window window in Application.Current.Windows)
                            {
                                if (window is DisplayWindow)
                                {
                                    window.Close();

                                    DisplayWindow displayWindow = new DisplayWindow(type, param);
                                    displayWindow.Show();
                                    displayWindow.MaximizeToSecondaryMonitor();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is DisplayWindow)
                    {
                        window.Close();
                    }
                }

                DisplayWindow displayWindow = new DisplayWindow(type, param)
                {
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
                    WindowState = System.Windows.WindowState.Maximized,
                    ResizeMode = ResizeMode.NoResize
                };
                displayWindow.Show();
            }
        }

        private void textTemplateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox textTemplate = sender as ComboBox;
            if(textTemplate.SelectedIndex > -1)
            {
                ShowTextDisplay((textTemplate.SelectedItem as CustomComboBoxItem).Value);
            }
        }

        private async void ShowTextDisplay(object selectedValue)
        {
            if (selectedValue != null)
            {
                DataRow textTemplateData = await dbCon.GetDataRow("TextTemplate", new DatabaseConnector.DataRow("TextTemplateId", selectedValue));
                if (textTemplateData.HasData && textTemplateData.Error == ERROR.NoError)
                {
                    textDisplayedTextBox.Text = textTemplateData.Get("TextTemplate").ToString();
                    rotateSecondsUpDown.Value = (int)textTemplateData.Get("RotateInSeconds");
                }
            }
        }

        private void showTextBtn_Click(object sender, RoutedEventArgs e)
        {
            bool IsFulfill = true;
            if (String.IsNullOrWhiteSpace(flightNoTextBox.Text))
            {
                flightNoTextBox.BorderBrush = Brushes.Red;
                flightNoTextBox.GotFocus += FlightNoTextBox_GotFocus;
                IsFulfill = false;
            }
            if(departureDatePicker.SelectedDate == null)
            {
                departureDatePicker.BorderBrush = Brushes.Red;
                departureDatePicker.GotFocus += DepartureDatePicker_GotFocus;
                IsFulfill = false;
            }
            if(departureTimePicker.Value == null)
            {
                departureTimePicker.BorderBrush = Brushes.Red;
                departureTimePicker.GotFocus += DepartureTimePicker_GotFocus;
                IsFulfill = false;
            }
            if(rotateSecondsUpDown.Value == null)
            {
                rotateSecondsUpDown.BorderBrush = Brushes.Red;
                rotateSecondsUpDown.GotFocus += RotateSecondsUpDown_GotFocus;
                IsFulfill = false;
            }
            if (String.IsNullOrWhiteSpace(textDisplayedTextBox.Text))
            {
                textDisplayedTextBox.BorderBrush = Brushes.Red;
                textDisplayedTextBox.GotFocus += TextDisplayedTextBox_GotFocus;
                IsFulfill = false;
            }
            if (aircraftConfigComboBox.SelectedValue == null || aircraftConfigComboBox.SelectedValue.Equals(String.Empty))
            {
                (aircraftConfigComboBox.Parent as Border).BorderBrush = Brushes.Red;
                aircraftConfigComboBox.GotFocus += AircraftConfigComboBox_GotFocus;
                IsFulfill = false;
            }
            if (frontDoorCheckBox.IsChecked == false && rearDoorCheckBox.IsChecked == false)
            {
                frontDoorCheckBox.BorderBrush = Brushes.Red;
                rearDoorCheckBox.BorderBrush = Brushes.Red;
                frontDoorCheckBox.GotFocus += FrontDoorCheckBox_GotFocus;
                rearDoorCheckBox.GotFocus += RearDoorCheckBox_GotFocus;
                IsFulfill = false;
            }
            //if (originComboBox.SelectedValue == null || originComboBox.SelectedValue.Equals(string.Empty))
            //{
            //    (originComboBox.Parent as Border).BorderBrush = Brushes.Red;
            //    originComboBox.GotFocus += OriginComboBox_GotFocus;
            //    IsFulfill = false;
            //}
            //if (destinationComboBox.SelectedValue == null || destinationComboBox.SelectedValue.Equals(string.Empty))
            //{
            //    (destinationComboBox.Parent as Border).BorderBrush = Brushes.Red;
            //    destinationComboBox.GotFocus += DestinationComboBox_GotFocus;
            //    IsFulfill = false;
            //}
            if (!IsFulfill) return;
            Guid TextTemplateId = (textTemplateComboBox.SelectedValue == null || 
                !textTemplateComboBox.SelectedValue.Equals(String.Empty)) ? 
                new Guid(textTemplateComboBox.SelectedValue.ToString()) : Guid.Empty;
            ShowText showText = new ShowText
            {
                TextTemplateId = TextTemplateId,
                FlightNo = flightNoTextBox.Text,
                OriginCode = (originComboBox.SelectedItem as CustomComboBoxItem).Value.ToString(),
                DestinationCode = (destinationComboBox.SelectedItem as CustomComboBoxItem).Value.ToString(),
                OriginName = (originComboBox.SelectedItem as CustomComboBoxItem).Text,
                DestinationName = (destinationComboBox.SelectedItem as CustomComboBoxItem).Text,
                DepartureDate = (DateTime)departureDatePicker.SelectedDate,
                DepartureTime = ((DateTime)departureTimePicker.Value).TimeOfDay,
                RotateInSeconds = (int)rotateSecondsUpDown.Value,
                TextTemplate = textDisplayedTextBox.Text
            };
            Guid aircraftConfigId = new Guid(aircraftConfigComboBox.SelectedValue.ToString());
            ShowWindow(DISPLAY_TYPE.TEXT, showText, aircraftConfigId, frontDoorCheckBox.IsChecked, rearDoorCheckBox.IsChecked);
        }

        private void DestinationComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((sender as ComboBox).Parent as Border).ClearValue(BorderBrushProperty);
            (sender as ComboBox).GotFocus -= DestinationComboBox_GotFocus;
        }

        private void OriginComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((sender as ComboBox).Parent as Border).ClearValue(BorderBrushProperty);
            (sender as ComboBox).GotFocus -= OriginComboBox_GotFocus;
        }

        private void RotateSecondsUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as IntegerUpDown).ClearValue(IntegerUpDown.BorderBrushProperty);
            (sender as IntegerUpDown).GotFocus -= RotateSecondsUpDown_GotFocus;
        }

        private void TextDisplayedTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).ClearValue(BorderBrushProperty);
            (sender as TextBox).GotFocus -= TextDisplayedTextBox_GotFocus;
        }

        private void DisplayMessageTimePicker_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TimePicker).ClearValue(BorderBrushProperty);
            (sender as TimePicker).GotFocus -= DisplayMessageTimePicker_GotFocus;
        }

        private void DepartureTimePicker_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TimePicker).ClearValue(TimePicker.BorderBrushProperty);
            (sender as TimePicker).GotFocus -= DepartureTimePicker_GotFocus;
        }

        private void DepartureDatePicker_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as DatePicker).ClearValue(TimePicker.BorderBrushProperty);
            (sender as DatePicker).GotFocus -= DepartureDatePicker_GotFocus;
        }

        private void FlightNoTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).ClearValue(BorderBrushProperty);
            (sender as TextBox).GotFocus -= FlightNoTextBox_GotFocus;
        }
        private void RearDoorCheckBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).ClearValue(CheckBox.BorderBrushProperty);
            (sender as CheckBox).GotFocus -= RearDoorCheckBox_GotFocus;
            frontDoorCheckBox.ClearValue(CheckBox.BorderBrushProperty);
            frontDoorCheckBox.GotFocus -= FrontDoorCheckBox_GotFocus;
        }

        private void FrontDoorCheckBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).ClearValue(CheckBox.BorderBrushProperty);
            (sender as CheckBox).GotFocus -= FrontDoorCheckBox_GotFocus;
            rearDoorCheckBox.ClearValue(CheckBox.BorderBrushProperty);
            rearDoorCheckBox.GotFocus -= RearDoorCheckBox_GotFocus;
        }

        private void AircraftConfigComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((sender as ComboBox).Parent as Border).ClearValue(ComboBox.BorderBrushProperty);
            (sender as ComboBox).GotFocus -= AircraftConfigComboBox_GotFocus;
        }

        private void showSeatMapBtn_Click(object sender, RoutedEventArgs e)
        {
            bool isReturn = false;
            if (aircraftConfigComboBox.SelectedValue == null || aircraftConfigComboBox.SelectedValue.Equals(String.Empty))
            {
                (aircraftConfigComboBox.Parent as Border).BorderBrush = Brushes.Red;
                aircraftConfigComboBox.GotFocus += AircraftConfigComboBox_GotFocus;
                isReturn = true;
            }
            if (frontDoorCheckBox.IsChecked == false && rearDoorCheckBox.IsChecked == false)
            {
                frontDoorCheckBox.BorderBrush = Brushes.Red;
                rearDoorCheckBox.BorderBrush = Brushes.Red;
                frontDoorCheckBox.GotFocus += FrontDoorCheckBox_GotFocus;
                rearDoorCheckBox.GotFocus += RearDoorCheckBox_GotFocus;
                isReturn = true;
            }
            if (isReturn) return;
            Guid aircraftConfigId = new Guid(aircraftConfigComboBox.SelectedValue.ToString());
            ShowWindow(DISPLAY_TYPE.SEATMAPS, null, aircraftConfigId, frontDoorCheckBox.IsChecked, rearDoorCheckBox.IsChecked);
        }

        private async void aircraftConfigComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(aircraftConfigComboBox.SelectedValue != null)
            {
                DataRow aircraftConfigData = await dbCon.GetDataRow("AircraftConfiguration", new DataRow("AircraftConfigurationId", aircraftConfigComboBox.SelectedValue));
                if(aircraftConfigData.HasData && aircraftConfigData.Error == ERROR.NoError)
                {
                    bool frontDoorUsingFlag = (bool)aircraftConfigData.Get("FrontDoorBoardingFlag");
                    bool rearDoorUsingFlag = (bool)aircraftConfigData.Get("RearDoorBoardingFlag");

                    frontDoorCheckBox.IsChecked = frontDoorUsingFlag;
                    rearDoorCheckBox.IsChecked = rearDoorUsingFlag;
                }
            }
        }
    }
}
