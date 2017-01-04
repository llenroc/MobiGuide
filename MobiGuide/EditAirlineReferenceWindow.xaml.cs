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

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for EditAirlineReferenceWindow.xaml
    /// </summary>
    public partial class EditAirlineReferenceWindow : Window
    {
        ResourceDictionary res = Application.Current.Resources;
        static string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
        public EditAirlineReferenceWindow()
        {
            InitializeComponent();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {

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

            fontColorPicker.SelectedColor = Colors.Black;
            backgroundColorPicker.SelectedColor = Colors.White;
            lineColorPicker.SelectedColor = Colors.Red;
            seatColorPicker.SelectedColor = Colors.Red;
            ///end of setup default UIs

            //get Airline Reference
            Dictionary<string, object> airlineRef = await getDataRow("AirlineReference", new Dictionary<string, object>()
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

            if (!String.IsNullOrEmpty(airlineRef["FontSize"].ToString()))
            {
                fontSizeComboBox.SelectedValue = (int)airlineRef["FontSize"];
            }

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
    }
}
