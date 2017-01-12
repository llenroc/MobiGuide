﻿using DatabaseConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for EditLanguageReferenceWindow.xaml
    /// </summary>
    public partial class EditLanguageReferenceWindow : Window
    {
        DBConnector dbCon = new DBConnector();
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        public EditLanguageReferenceWindow()
        {
            InitializeComponent();
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            DataRow data = new DataRow(
                "LanguageName", languageNameTextBox.Text,
                "StatusCode", statusComboBox.SelectedValue,
                "CommitBy", Application.Current.Resources["UserAccountId"],
                "CommitDateTime", DateTime.Now
                );
            if(await SaveEditLanguageReference(data, new DataRow("LanguageCode", languageCodeComboBox.SelectedValue))){
                MessageBox.Show(String.Format("Update [{0}] Language Reference Successfully", languageCodeComboBox.SelectedValue), "SUCCESS");
                GetLanguageReferenceData(languageCodeComboBox.SelectedValue.ToString());
            } else
            {
                MessageBox.Show(String.Format("Failed to Update [{0}] Language Reference", languageCodeComboBox.SelectedValue), "ERROR");
            }
            saveBtn.IsEnabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            statusComboBox.Items.Add(new ComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new ComboBoxItem { Text = "Inactive", Value = "I" });

            FetchLanguageList();
        }

        private async void FetchLanguageList()
        {
            languageCodeComboBox.Items.Clear();
            DataList list = await dbCon.getDataList("LanguageReference", null, "ORDER BY LanguageCode");

            languageCodeComboBox.Items.Add(new ComboBoxItem { Text = "---", Value = "NOSEL" });
            if(list.Error == ERROR.NoError && list.HasData)
            {
                foreach(DataRow row in list)
                {
                    languageCodeComboBox.Items.Add(new ComboBoxItem { Text = row.Get("LanguageCode").ToString(), Value = row.Get("LanguageCode").ToString() });
                }
                languageCodeComboBox.SelectionChanged += RemoveNotSelectOptionItem;
                languageCodeComboBox.SelectionChanged += LanguageCodeComboBox_SelectionChanged;

            } else
            {
                MessageBox.Show("Cannot Get Language Code List", "ERROR");
            }
        }

        private void LanguageCodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetLanguageReferenceData(languageCodeComboBox.SelectedValue.ToString());
        }

        private async void GetLanguageReferenceData(string languageCode)
        {
            DataRow languageRef = await dbCon.getDataRow("LanguageReference", new DataRow("LanguageCode", languageCode));
            if(languageRef.HasData && languageRef.Error == ERROR.NoError)
            {
                languageNameTextBox.Text = languageRef.Get("LanguageName") != DBNull.Value ? languageRef.Get("LanguageName").ToString() : "NULL";
                statusComboBox.SelectedValue = languageRef.Get("StatusCode") != DBNull.Value ? languageRef.Get("StatusCode") : "A";
                commitDateTimeTextBlockValue.Text = languageRef.Get("CommitDateTime") != DBNull.Value ? languageRef.Get("CommitDateTime").ToString() : "NULL";
                DataRow commitBy = await dbCon.getDataRow("UserAccount", new DataRow("UserAccountId", languageRef.Get("CommitBy")));
                if (commitBy.HasData && commitBy.Error == ERROR.NoError)
                    commitByTextBlockValue.Text = String.Format("{0} {1}", commitBy.Get("FirstName"), commitBy.Get("LastName"));
                else commitByTextBlockValue.Text = "NULL";
            }
        }

        private async Task<bool> SaveEditLanguageReference(DataRow data, DataRow conditions)
        {
            return await dbCon.updateDataRow("LanguageReference", data, conditions);
            
        }

        private void RemoveNotSelectOptionItem(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).Items.Count > 0) (sender as ComboBox).Items.RemoveAt(0);
            (sender as ComboBox).SelectionChanged -= RemoveNotSelectOptionItem;
            languageNameTextBlock.Visibility = Visibility.Visible;
            languageNameTextBox.Visibility = Visibility.Visible;
            statusTextBlock.Visibility = Visibility.Visible;
            statusComboBox.Visibility = Visibility.Visible;
            commitByTextBlock.Visibility = Visibility.Visible;
            commitByTextBlockValue.Visibility = Visibility.Visible;
            commitDateTimeTextBlock.Visibility = Visibility.Visible;
            commitDateTimeTextBlockValue.Visibility = Visibility.Visible;
        }

        private void languageNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(languageNameTextBox.Text)) saveBtn.IsEnabled = true;
            else saveBtn.IsEnabled = false;
        }
    }
}