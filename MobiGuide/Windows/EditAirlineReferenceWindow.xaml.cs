using Microsoft.Win32;
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
using System.Windows.Shapes;
using CustomExtensions;
using System.IO;
using DatabaseConnector;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for EditAirlineReferenceWindow.xaml
    /// </summary>
    public partial class EditAirlineReferenceWindow : Window
    {
        ResourceDictionary res = Application.Current.Resources;
        DataRow airlineRef = new DataRow();
        private ImageUploadStatus largeLogoStatus = ImageUploadStatus.Remain;
        private ImageUploadStatus smallLogoStatus = ImageUploadStatus.Remain;
        private string largeLogoPath = String.Empty;
        private string smallLogoPath = String.Empty;
        DBConnector dbCon = new DBConnector();
        public EditAirlineReferenceWindow()
        {
            InitializeComponent();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            DataRow airlineRef = gatherDataToUpdate();
            DataRow condition = new DataRow();
            condition.Set("AirlineCode", res["AirlineCode"]);
            bool updateResult = await dbCon.UpdateDataRow("AirlineReference", airlineRef, condition);
            switch (largeLogoStatus)
            {
                case ImageUploadStatus.New:
                    if (!await dbCon.UpdateBlobData("AirlineReference", "AirlineLogoLarge",
                        largeLogoPath, condition))
                        MessageBox.Show("Failed to update Airline Large Logo", "ERROR");
                    break;
                case ImageUploadStatus.Remove:
                    if (!await dbCon.UpdateBlobData("AirlineReference", "AirlineLogoLarge",
                        null, condition))
                        MessageBox.Show("Failed to remove Airline Large Logo", "ERROR");
                    break;
            }
            switch (smallLogoStatus)
            {
                case ImageUploadStatus.New:
                    if (!await dbCon.UpdateBlobData("AirlineReference", "AirlineLogoSmall",
                        smallLogoPath, condition))
                        MessageBox.Show("Failed to update Airline Small Logo", "ERROR");
                    break;
                case ImageUploadStatus.Remove:
                    if (!await dbCon.UpdateBlobData("AirlineReference", "AirlineLogoSmall",
                        null, condition))
                        MessageBox.Show("Failed to remove Airline Small Logo", "ERROR");
                    break;
            }
            if (updateResult)
            {
                MessageBox.Show("Update Airline Reference Successfully", "SUCCESS");
                DialogResult = true;
                this.Close();
            } else
            {
                MessageBox.Show("Failed to update Airline Reference", "ERROR");
                saveBtn.IsEnabled = true;
            }
        }

        private DataRow gatherDataToUpdate()
        {
            DataRow data = new DataRow();
            data.Set("AirlineName", alNameTxtBox.Text);
            data.Set("FontName", fontNameComboBox.SelectedValue.ToString());
            data.Set("FontSize", fontSizeUpDown.Value);
            data.Set("FontColor", ((Color)fontColorPicker.SelectedColor).GetInteger());
            data.Set("BackGroundColor", ((Color)backgroundColorPicker.SelectedColor).GetInteger());
            data.Set("BackGroundColor2", ((Color)backgroundColorPicker2.SelectedColor).GetInteger());
            data.Set("LineColor", ((Color)lineColorPicker.SelectedColor).GetInteger());
            data.Set("SeatColor", ((Color)seatColorPicker.SelectedColor).GetInteger());
            data.Set("ShowGuidanceInSeconds", dispGuideTimeIntUpDown.Value);
            data.Set("StatusCode", statusCodeComboBox.SelectedValue);
            data.Set("CommitBy", res["UserAccountId"]);
            data.Set("CommitDateTime", DateTime.Now);
            return data;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void browseSrcLargeLogoBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog imgofDialog = new OpenFileDialog();
            imgofDialog.Filter = "Image Files|*.jpg;*.bmp;*.png";
            imgofDialog.FilterIndex = 1;

            if (imgofDialog.ShowDialog() == true)
            {
                string filePath = imgofDialog.FileName;
                srcLargeLogoTxtBox.Text = filePath.Length >= 30 ? filePath.Shorten() : filePath;

                alLargeLogoImg.Source = new BitmapImage(new Uri(filePath));
                largeLogoPath = filePath;

                removeLargeLogoBtn.Visibility = Visibility.Visible;
                switch (largeLogoStatus)
                {
                    case ImageUploadStatus.Remain:
                        largeLogoStatus = ImageUploadStatus.New;
                        break;
                    case ImageUploadStatus.New:
                        largeLogoStatus = ImageUploadStatus.Remain;
                        break;
                    case ImageUploadStatus.Remove:
                        largeLogoStatus = ImageUploadStatus.New;
                        break;
                }
            }
        }

        private void browseSrcSmallLogoBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog imgofDialog = new OpenFileDialog();
            imgofDialog.Filter = "Image Files|*.jpg;*.bmp;*.png";
            imgofDialog.FilterIndex = 1;

            if (imgofDialog.ShowDialog() == true)
            {
                string filePath = imgofDialog.FileName;
                srcSmallLogoTxtBox.Text = filePath.Length >= 30 ? filePath.Shorten() : filePath;

                alSmallLogoImg.Source = new BitmapImage(new Uri(filePath));
                smallLogoPath = filePath;

                removeSmallLogoBtn.Visibility = Visibility.Visible;
                switch (smallLogoStatus)
                {
                    case ImageUploadStatus.Remain:
                        smallLogoStatus = ImageUploadStatus.New;
                        break;
                    case ImageUploadStatus.New:
                        smallLogoStatus = ImageUploadStatus.Remain;
                        break;
                    case ImageUploadStatus.Remove:
                        smallLogoStatus = ImageUploadStatus.New;
                        break;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            displayInfo();
        }

        private async void displayInfo()
        {
            //display Airline Code
            string airlineCode = res["AirlineCode"].ToString();
            alCodeTxtBlock.Text = airlineCode;

            /// setup default UIs

            statusCodeComboBox.Items.Add(new CustomComboBoxItem { Text = "Active", Value = "A" });
            statusCodeComboBox.Items.Add(new CustomComboBoxItem { Text = "Inactive", Value = "I" });
            ///end of setup default UIs

            //get Airline Reference
            DataRow condition = new DataRow();
            condition.Set("AirlineCode", res["AirlineCode"]);
            airlineRef = await dbCon.GetDataRow("AirlineReference", condition);

            alNameTxtBox.Text = airlineRef.Get("AirlineName").ToString();

            //set font name combobox selecteditem 
            string defaultFontFamily = FontFamily.FamilyNames.Select(fontName => fontName.Value).ToList()[0];
            string selectedFontFamily = airlineRef.Get("FontName").ToString();
            if (String.IsNullOrEmpty(selectedFontFamily))
            {
                foreach (FontFamily fontFamily in fontNameComboBox.Items)
                {
                    if (fontFamily.Source == defaultFontFamily)
                    {
                        fontNameComboBox.SelectedItem = fontFamily;
                    }
                }
            }
            else
            {
                foreach (FontFamily fontFamily in fontNameComboBox.Items)
                {
                    if (fontFamily.Source == selectedFontFamily)
                    {
                        fontNameComboBox.SelectedItem = fontFamily;
                    }
                }
            }

            //font size combobox
            if (!String.IsNullOrEmpty(airlineRef.Get("FontSize").ToString()))
            {
                fontSizeUpDown.Value = (int)airlineRef.Get("FontSize");
            }

            //colors
            fontColorPicker.SelectedColor = ((int)airlineRef.Get("FontColor")).GetColor();
            backgroundColorPicker.SelectedColor = ((int)airlineRef.Get("BackGroundColor")).GetColor();
            backgroundColorPicker2.SelectedColor = ((int)airlineRef.Get("BackGroundColor2")).GetColor();
            lineColorPicker.SelectedColor = ((int)airlineRef.Get("LineColor")).GetColor();
            seatColorPicker.SelectedColor = ((int)airlineRef.Get("SeatColor")).GetColor();

            dispGuideTimeIntUpDown.Value = (int)airlineRef.Get("ShowGuidanceInSeconds");
            statusCodeComboBox.SelectedValue = airlineRef.Get("StatusCode").ToString();
            commitTimeTxtBlock.Text = airlineRef.Get("CommitDateTime").ToString();

            //display CommitBy full name
            commitByTxtBlock.Text = await dbCon.GetFullNameFromUid(airlineRef.Get("CommitBy").ToString());

            if (airlineRef.Get("AirlineLogoLarge") != DBNull.Value)
            {
                alLargeLogoImg.Source = ((object)airlineRef.Get("AirlineLogoLarge")).BlobToSource();
                removeLargeLogoBtn.Visibility = Visibility.Visible;

            }
            else
            {
                alLargeLogoImg.Source = new BitmapImage(new Uri(@"NoImg.jpg", UriKind.RelativeOrAbsolute));
            }
            if (airlineRef.Get("AirlineLogoSmall") != DBNull.Value)
            {
                alSmallLogoImg.Source = ((object)airlineRef.Get("AirlineLogoSmall")).BlobToSource();
                removeSmallLogoBtn.Visibility = Visibility.Visible;
            }
            else
            {
                alSmallLogoImg.Source = new BitmapImage(new Uri(@"NoImg.jpg", UriKind.RelativeOrAbsolute));
            }
        }

        private enum ImageUploadStatus
        {
            Remove,
            New,
            Remain
        }

        private void removeLargeLogoBtn_Click(object sender, RoutedEventArgs e)
        {
            removeLargeLogoBtn.Visibility = Visibility.Hidden;
            alLargeLogoImg.Source = new BitmapImage(new Uri(@"NoImg.jpg", UriKind.RelativeOrAbsolute));
            srcLargeLogoTxtBox.Text = String.Empty;
            switch (largeLogoStatus)
            {
                case ImageUploadStatus.Remain:
                    largeLogoStatus = ImageUploadStatus.Remove;
                    break;
                case ImageUploadStatus.New:
                    largeLogoStatus = ImageUploadStatus.Remain;
                    break;
            }
                    
        }

        private void removeSmallLogoBtn_Click(object sender, RoutedEventArgs e)
        {
            removeSmallLogoBtn.Visibility = Visibility.Hidden;
            alSmallLogoImg.Source = new BitmapImage(new Uri(@"NoImg.jpg", UriKind.RelativeOrAbsolute));
            srcSmallLogoTxtBox.Text = String.Empty;
            switch (smallLogoStatus)
            {
                case ImageUploadStatus.Remain:
                    smallLogoStatus = ImageUploadStatus.Remove;
                    break;
                case ImageUploadStatus.New:
                    smallLogoStatus = ImageUploadStatus.Remain;
                    break;
            }
        }

        private void backgroundColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if(backgroundColorPicker.SelectedColor != null)
            {
                if(backgroundColorPicker2.SelectedColor != null && backgroundColorPicker2.SelectedColor != Colors.Transparent)
                {
                    LinearGradientBrush gradiantBrush =
                        new LinearGradientBrush();
                    gradiantBrush.StartPoint = new Point(0, 0);
                    gradiantBrush.EndPoint = new Point(1, 1);
                    gradiantBrush.GradientStops.Add(
                        new GradientStop((Color)backgroundColorPicker.SelectedColor, 0.3));
                    gradiantBrush.GradientStops.Add(
                        new GradientStop((Color)backgroundColorPicker2.SelectedColor, 1.0));
                    backgroundPreview.Fill = gradiantBrush;
                } else
                {
                    backgroundPreview.Fill = new SolidColorBrush((Color)backgroundColorPicker.SelectedColor);
                }
            }
        }
    }
}
