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
using System.Windows.Shapes;
using DatabaseConnector;
using Properties;
using System.Diagnostics;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for AircraftTypeWIndow.xaml
    /// </summary>
    public partial class AircraftConfigurationWindow : Window
    {
        DBConnector dbCon = new DBConnector();
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        public AircraftConfigurationWindow()
        {
            InitializeComponent();

            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Any", Value = "" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Inactive", Value = "I" });
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }
        private async void DoSearch()
        {
            string aircraftConfigCode = aircraftConfigCodeTextBox.Text;
            string aircraftConfigName = aircraftConfigNameTextBox.Text;
            string status = statusComboBox.SelectedValue.ToString();
            string aircraftTypeCode = aircraftTypeComboBox.SelectedValue.ToString();

            DataList list = await dbCon.GetDataList("AircraftConfiguration", null,
                String.Format("WHERE AircraftConfigurationCode LIKE '%{0}%' AND AircraftConfigurationName LIKE '%{1}%' AND StatusCode LIKE '%{2}%' AND AircraftTypeCode LIKE '%{3}%' COLLATE SQL_Latin1_General_CP1_CI_AS ORDER BY AircraftTypeCode",
                aircraftConfigCode, aircraftConfigName, status, aircraftTypeCode));
            if (list.HasData && list.Error == ERROR.NoError)
            {
                List<AircraftConfiguration> _itemSource = new List<AircraftConfiguration>();
                foreach (DataRow row in list)
                {
                    AircraftConfiguration aircraftConfiguration = new AircraftConfiguration
                    {
                        AircraftConfigurationId = (Guid)row.Get("AircraftConfigurationId"),
                        AirlineCode = row.Get("AirlineCode").ToString(),
                        AircraftConfigurationCode = row.Get("AircraftConfigurationCode").ToString(),
                        AircraftConfigurationName = row.Get("AircraftConfigurationName").ToString(),
                        AisleX = (int)row.Get("AisleX"),
                        FrontDoorX = (int)row.Get("FrontDoorX"),
                        FrontDoorY = (int)row.Get("FrontDoorY"),
                        RearDoorX = (int)row.Get("RearDoorX"),
                        RearDoorY = (int)row.Get("RearDoorY"),
                        FrontDoorBoardingFlag = (bool)row.Get("FrontDoorBoardingFlag"),
                        RearDoorBoardingFlag = (bool)row.Get("RearDoorBoardingFlag"),
                        Status = row.Get("StatusCode").ToString()
                    };
                    DataRow aircraftTypeData = await dbCon.GetDataRow("AircraftTypeReference", new DataRow("AircraftTypeCode", row.Get("AircraftTypeCode")));
                    if(aircraftTypeData.HasData && aircraftTypeData.Error == ERROR.NoError)
                    {
                        AircraftType aircraftType = new AircraftType
                        {
                            AircraftTypeCode = aircraftTypeData.Get("AircraftTypeCode").ToString(),
                            AircraftTypeName = aircraftTypeData.Get("AircraftTypeName").ToString(),
                            Status = aircraftTypeData.Get("StatusCode").ToString()
                        };
                        aircraftConfiguration.AircraftType = aircraftType;
                    }
                    _itemSource.Add(aircraftConfiguration);
                }
                aircraftConfigDataGrid.ItemsSource = _itemSource;
            }
            else
            {
                aircraftConfigDataGrid.ItemsSource = null;
                aircraftConfigDataGrid.SelectedIndex = -1;
            }
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void textTemplateDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedIndex != -1) editBtn.IsEnabled = true;
            else editBtn.IsEnabled = false;
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            aircraftConfigCodeTextBox.Text = String.Empty;
            aircraftConfigNameTextBox.Text = String.Empty;
            statusComboBox.SelectedIndex = 0;
            aircraftConfigDataGrid.ItemsSource = null;
            aircraftConfigDataGrid.SelectedIndex = -1;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            aircraftTypeComboBox.Items.Add(new CustomComboBoxItem { Text = "Any", Value = "" });

            DataList aircraftTypeList = await dbCon.GetDataList("AircraftTypeReference", null);
            if(aircraftTypeList.HasData && aircraftTypeList.Error == ERROR.NoError)
            {
                foreach(DataRow row in aircraftTypeList)
                {
                    aircraftTypeComboBox.Items.Add(new CustomComboBoxItem { Text = row.Get("AircraftTypeCode").ToString(), Value = row.Get("AircraftTypeCode").ToString() });
                }
            }
        }
    }
}