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
using System.Diagnostics;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for EditAirlineReferenceWindow.xaml
    /// </summary>
    public partial class EditAirlineReferenceWindow : Window
    {
        ResourceDictionary res = Application.Current.Resources;
        private Dictionary<string, object> airlineRef = new Dictionary<string, object>();
        static string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
        private ImageUploadStatus largeLogoStatus = ImageUploadStatus.Remain;
        private ImageUploadStatus smallLogoStatus = ImageUploadStatus.Remain;
        private string largeLogoPath = String.Empty;
        private string smallLogoPath = String.Empty;
        public EditAirlineReferenceWindow()
        {
            InitializeComponent();
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            gatherDataToUpdate();
            bool result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        //update airline reference info
                        string query = String.Format("UPDATE AirlineReference SET");
                        for(int i = 0; i < airlineRef.Count; i++)
                        {
                            if(airlineRef.ElementAt(i).Key != "AirlineLogoSmall" && 
                                airlineRef.ElementAt(i).Key != "AirlineLogoLarge" &&
                                airlineRef.ElementAt(i).Key != "AirlineCode")
                            {
                                query += String.Format(" {0} = '{1}'", airlineRef.ElementAt(i).Key, airlineRef.ElementAt(i).Value);
                                if (i < airlineRef.Count - 1) query += ",";
                            }
                        }
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        cmd.ExecuteNonQuery();

                        //upload large logo
                        switch (largeLogoStatus)
                        {
                            case ImageUploadStatus.New:
                                query = String.Format("UPDATE AirlineReference ");
                                query += String.Format("SET AirlineLogoLarge = ");
                                query += String.Format("(SELECT BulkColumn FROM OPENROWSET(BULK  '{0}', SINGLE_BLOB) AS x) ", largeLogoPath);
                                query += String.Format("WHERE AirlineCode = '{0}'", res["AirlineCode"].ToString());
                                break;
                            case ImageUploadStatus.Remove:
                                query = String.Format("UPDATE AirlineReference ");
                                query += String.Format("SET AirlineLogoLarge = NULL ");
                                query += String.Format("WHERE AirlineCode = '{0}'", res["AirlineCode"].ToString());
                                break;
                        }
                        cmd = new SqlCommand(query, con);
                        cmd.ExecuteNonQuery();

                        //upload small logo
                        switch (smallLogoStatus)
                        {
                            case ImageUploadStatus.New:
                                query = String.Format("UPDATE AirlineReference ");
                                query += String.Format("SET AirlineLogoSmall = ");
                                query += String.Format("(SELECT BulkColumn FROM OPENROWSET(BULK  '{0}', SINGLE_BLOB) AS x) ", smallLogoPath);
                                query += String.Format("WHERE AirlineCode = '{0}'", res["AirlineCode"].ToString());
                                break;
                            case ImageUploadStatus.Remove:
                                query = String.Format("UPDATE AirlineReference ");
                                query += String.Format("SET AirlineLogoSmall = NULL ");
                                query += String.Format("WHERE AirlineCode = '{0}'", res["AirlineCode"].ToString());
                                break;
                        }
                        cmd = new SqlCommand(query, con);
                        cmd.ExecuteNonQuery();

                        con.Close();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR");
                    return false;
                }
            });
            if (result)
            {
                this.Hide();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        MobiGuide.UserInfo userInfo = (MobiGuide.UserInfo)window.DataContext;
                        userInfo.AirlineName = airlineRef["AirlineName"].ToString();
                        (window as MainWindow).DataContext = userInfo;
                    }
                }
                MessageBox.Show("Update Airline Reference Successfully", "SUCCESS");
                this.Close();
            } else
            {
                saveBtn.IsEnabled = true;
            }
        }

        private void gatherDataToUpdate()
        {
            airlineRef["AirlineName"] = alNameTxtBox.Text;
            airlineRef["FontName"] = fontNameComboBox.SelectedValue;
            airlineRef["FontSize"] = fontSizeComboBox.SelectedValue;
            airlineRef["FontColor"] = ((Color)fontColorPicker.SelectedColor).GetInteger();
            airlineRef["BackGroundColor"] = ((Color)backgroundColorPicker.SelectedColor).GetInteger();
            airlineRef["LineColor"] = ((Color)lineColorPicker.SelectedColor).GetInteger();
            airlineRef["SeatColor"] = ((Color)seatColorPicker.SelectedColor).GetInteger();
            airlineRef["ShowGuidanceInSeconds"] = dispGuideTimeIntUpDown.Value;
            airlineRef["StatusCode"] = statusCodeComboBox.SelectedValue;
            airlineRef["CommitBy"] = res["UserAccountId"];
            airlineRef["CommitDateTime"] = DateTime.Now;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //display Airline Code
            string airlineCode = res["AirlineCode"].ToString();
            alCodeTxtBlock.Text = airlineCode;

            /// setup default UIs
            int[] fontSizeStep = { 6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 36, 48, 72 };
            foreach(int fontSize in fontSizeStep)
            {
                fontSizeComboBox.Items.Add(new ComboBoxItem { Text = fontSize + " px", Value = fontSize });
            }

            statusCodeComboBox.Items.Add(new ComboBoxItem { Text = "Active", Value = "A" });
            statusCodeComboBox.Items.Add(new ComboBoxItem { Text = "Inactive", Value = "I" });
            ///end of setup default UIs

            //get Airline Reference
            airlineRef = await getDataRow("AirlineReference", new Dictionary<string, object>()
            {
                { "AirlineCode", airlineCode }
            });

            alNameTxtBox.Text = airlineRef["AirlineName"].ToString();

            //set font name combobox selecteditem 
            string defaultFontFamily = FontFamily.FamilyNames.Select(fontName => fontName.Value).ToList()[0];
            string selectedFontFamily = airlineRef["FontName"].ToString();
            if (String.IsNullOrEmpty(selectedFontFamily))
            {
                foreach (FontFamily fontFamily in fontNameComboBox.Items)
                {
                    if (fontFamily.Source == defaultFontFamily)
                    {
                        fontNameComboBox.SelectedItem = fontFamily;
                    }
                }
            } else
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
            if (!String.IsNullOrEmpty(airlineRef["FontSize"].ToString()))
            {
                fontSizeComboBox.SelectedValue = (int)airlineRef["FontSize"];
            }

            //colors
            fontColorPicker.SelectedColor = ((int)airlineRef["FontColor"]).GetColor();
            backgroundColorPicker.SelectedColor = ((int)airlineRef["BackGroundColor"]).GetColor();
            lineColorPicker.SelectedColor = ((int)airlineRef["LineColor"]).GetColor();
            seatColorPicker.SelectedColor = ((int)airlineRef["SeatColor"]).GetColor();

            dispGuideTimeIntUpDown.Value = (int)airlineRef["ShowGuidanceInSeconds"];
            statusCodeComboBox.SelectedValue = airlineRef["StatusCode"].ToString();
            commitTimeTxtBlock.Text = airlineRef["CommitDateTime"].ToString();
            
            //display CommitBy full name
            Dictionary<string, object> commitUser = await getDataRow("UserAccount", new Dictionary<string, object>() {
                { "UserAccountId", airlineRef["CommitBy"] }
            });
            if(commitUser.Count > 0)
            {
                commitByTxtBlock.Text = String.Format("{0} {1}", commitUser["FirstName"].ToString(), commitUser["LastName"].ToString());
            }

            if (airlineRef["AirlineLogoLarge"] != DBNull.Value)
            {
                alLargeLogoImg.Source = BlobToSource(airlineRef["AirlineLogoLarge"]);
                removeLargeLogoBtn.Visibility = Visibility.Visible;

            }
            if (airlineRef["AirlineLogoSmall"] != DBNull.Value)
            {
                alSmallLogoImg.Source = BlobToSource(airlineRef["AirlineLogoSmall"]);
                removeSmallLogoBtn.Visibility = Visibility.Visible;
            }
        }

        private static async Task<Dictionary<string, object>> getDataRow(string tableName, Dictionary<string, object> conditions)
        {
            Dictionary<string, object> result = await Task.Run(() =>
            {
                try
                {
                    using(SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = String.Format("SELECT * FROM {0}", tableName);
                        if(conditions.Count > 0)
                        {
                            query += " WHERE";
                            for(int i = 0; i < conditions.Count; i++)
                            {
                                query += String.Format(" {0} = '{1}'", conditions.ElementAt(i).Key, conditions.ElementAt(i).Value.ToString());
                                if (i < conditions.Count - 1) query += " AND";
                            }
                        }
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        row.Add(reader.GetName(i), reader.GetValue(i));
                                    }
                                }
                            }
                        }
                        return row;
                    }
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return new Dictionary<string, object>();
                }
            });
            return result;
        }

        private static ImageSource BlobToSource(object obj)
        {
            try
            {
                if (obj == null)
                {
                    return null;
                }
                else
                {
                    byte[] bArray = (byte[])obj;

                    BitmapImage biImg = new BitmapImage();
                    MemoryStream ms = new MemoryStream(bArray);
                    biImg.BeginInit();
                    biImg.StreamSource = ms;
                    biImg.EndInit();

                    ImageSource imgSrc = biImg as ImageSource;

                    return imgSrc;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
                return null;
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
    }
}
