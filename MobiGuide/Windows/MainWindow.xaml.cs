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

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ResourceDictionary res = Application.Current.Resources;
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
            } catch (Exception ex){
                MessageBox.Show("Unexpected Error Occurred! Please contact Administator.", "Error");
                this.Close();
            }
        }

        private async void displayInfo()
        {
            //Get Full Name of current user
            DBConnector dbCon = new DBConnector();
            DataRow userCondi = new DataRow();
            userCondi.Set("UserAccountId", res["UserAccountId"]);
            DataRow userInfo = await dbCon.getDataRow("UserAccount", userCondi);
            nameTxtBlock.Text = String.Format("{0} {1}", userInfo.Get("FirstName").ToString(), userInfo.Get("LastName").ToString());

            DataRow airlineRefCondi = new DataRow();
            airlineRefCondi.Set("AirlineCode", res["AirlineCode"]);
            DataRow airlineRef = await dbCon.getDataRow("AirlineReference", airlineRefCondi);
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
            if (editALRefWindow.DialogResult.HasValue && editALRefWindow.DialogResult.Value)
            {
                displayInfo();
            }
        }

        private void AddAirportReferenceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddAirportReferenceWindow addAirportReferenceWindow = new AddAirportReferenceWindow();
            addAirportReferenceWindow.ShowDialog();
        }

        private void EditAirportReferenceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditAirportReferenceWindow editAirportReferenceWindow = new EditAirportReferenceWindow();
            editAirportReferenceWindow.ShowDialog();
        }

        private void AddLanguageReference_Click(object sender, RoutedEventArgs e)
        {
            AddLanguageReferenceWindow addLanguageReferenceWindow = new AddLanguageReferenceWindow();
            addLanguageReferenceWindow.ShowDialog();
        }

        private void EditLanguageReference_Click(object sender, RoutedEventArgs e)
        {
            EditLanguageReferenceWindow editLanguageReferenceWindow = new EditLanguageReferenceWindow();
            editLanguageReferenceWindow.ShowDialog();
        }

        private void AddAirportTranslationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddAirportTranslationWindow addAirportTranslationWindow = new AddAirportTranslationWindow();
            addAirportTranslationWindow.ShowDialog();
        }

        private void EditAirportTranslationMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
