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
using System.Drawing;
using System.ComponentModel;
using DatabaseConnector;
using CustomExtensions;
using System.Diagnostics;
using Properties;
using MobiGuide.Helper;

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
                MessageBox.Show("Unexpected Error Occurred! Please contact Administator.", "Error");
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
            if (MessageBox.Show("Do you want to logout?", "Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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

                if (selectedAircraftConfig != String.Empty)
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

                if (selectedOrigin != String.Empty)
                    originComboBox.SelectedValue = selectedOrigin;
                else
                    originComboBox.SelectedIndex = 0;

                if (selectedDestination != String.Empty)
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
                if(selectedTextTemplate != String.Empty)
                    textTemplateComboBox.SelectedValue = selectedTextTemplate;
                else
                    textTemplateComboBox.SelectedIndex = 0;
            } else
            {
                textTemplateComboBox.Items.Add(new CustomComboBoxItem { Text = "No Data", Value = String.Empty });
                textTemplateComboBox.SelectedIndex = 0;
            }
        }

        private void showLogoBtn_Click(object sender, RoutedEventArgs e)
        {
            if(System.Windows.Forms.Screen.AllScreens.Count() > 1)
            {
                foreach(System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
                {
                    if(screen != System.Windows.Forms.Screen.AllScreens[0])
                    {
                        if (!EventHelper.IsWindowOpen<DisplayWindow>())
                        {
                            DisplayWindow displayWindow = new DisplayWindow();
                            displayWindow.Show();
                            displayWindow.MaximizeToSecondaryMonitor();
                        }else
                        {
                            foreach(Window window in Application.Current.Windows)
                            {
                                if(window is DisplayWindow)
                                {
                                    window.Close();

                                    DisplayWindow displayWindow = new DisplayWindow();
                                    displayWindow.Show();
                                    displayWindow.MaximizeToSecondaryMonitor();
                                }
                            }
                        }
                    }
                }
            } else
            {
                DisplayWindow displayWindow = new DisplayWindow
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    WindowState = WindowState.Maximized,
                    ResizeMode = ResizeMode.NoResize
                };
                displayWindow.Show();
            }
        }
    }
}
