﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DatabaseConnector;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for AircraftTypeWIndow.xaml
    /// </summary>
    public partial class AircraftTypeWindow : Window
    {
        private readonly DBConnector dbCon = new DBConnector();

        public AircraftTypeWindow()
        {
            InitializeComponent();

            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Any", Value = "" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Inactive", Value = "I" });
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }

        private async void DoSearch()
        {
            string aircraftTypeCode = aircraftTypeCodeTextBox.Text;
            string aircraftTypeName = aircraftTypeNameTextBox.Text;
            string status = statusComboBox.SelectedValue.ToString();

            DataList list = await dbCon.GetDataList("AircraftTypeReference", null,
                string.Format("WHERE AircraftTypeCode LIKE '%{0}%' AND AircraftTypeName LIKE '%{1}%' AND StatusCode LIKE '%{2}%' COLLATE SQL_Latin1_General_CP1_CI_AS ORDER BY AircraftTypeCode",
                aircraftTypeCode, aircraftTypeName, status));
            if (list.HasData && list.Error == ERROR.NoError)
            {
                List<AircraftType> _itemSource = new List<AircraftType>();
                foreach (DataRow row in list)
                {
                    AircraftType aircraftType = new AircraftType
                    {
                        AircraftTypeCode = row.Get("AircraftTypeCode").ToString(),
                        AircraftTypeName = row.Get("AircraftTypeName").ToString(),
                        StatusCode = row.Get("StatusCode").ToString()
                    };
                    _itemSource.Add(aircraftType);
                }
                aircraftTypeDataGrid.ItemsSource = _itemSource;
            }
            else
            {
                aircraftTypeDataGrid.ItemsSource = null;
                aircraftTypeDataGrid.SelectedIndex = -1;
            }
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            NewEditAircraftTypeWindow newEditAircraftTypeWindow = new NewEditAircraftTypeWindow();
            newEditAircraftTypeWindow.ShowDialog();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            int indexOfSelectedItem = aircraftTypeDataGrid.SelectedIndex;
            if (indexOfSelectedItem > -1)
            {
                string aircraftTypeCode = (aircraftTypeDataGrid.SelectedItem as AircraftType).AircraftTypeCode;
                NewEditAircraftTypeWindow editAircraftTypeWindow = new NewEditAircraftTypeWindow(aircraftTypeCode);
                editAircraftTypeWindow.ShowDialog();
                if (editAircraftTypeWindow.DialogResult.HasValue && editAircraftTypeWindow.DialogResult.Value)
                    DoSearch();
            }
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            aircraftTypeCodeTextBox.Text = string.Empty;
            aircraftTypeNameTextBox.Text = string.Empty;
            statusComboBox.SelectedIndex = 0;
            aircraftTypeDataGrid.ItemsSource = null;
            aircraftTypeDataGrid.SelectedIndex = -1;
        }

        private void aircraftTypeDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int indexOfSelectedItem = aircraftTypeDataGrid.SelectedIndex;
            if (indexOfSelectedItem > -1)
            {
                string aircraftTypeCode = (aircraftTypeDataGrid.SelectedItem as AircraftType).AircraftTypeCode;
                NewEditAircraftTypeWindow editAircraftTypeWindow = new NewEditAircraftTypeWindow(aircraftTypeCode);
                editAircraftTypeWindow.ShowDialog();
                if (editAircraftTypeWindow.DialogResult.HasValue && editAircraftTypeWindow.DialogResult.Value)
                    DoSearch();
            }
        }

        private void aircraftTypeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedIndex != -1) editBtn.IsEnabled = true;
            else editBtn.IsEnabled = false;
        }
    }
}
