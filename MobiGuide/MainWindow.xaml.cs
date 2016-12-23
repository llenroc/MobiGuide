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

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string connectionString = String.Empty;
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
            connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
            try
            {
                //display user information
                string firstName = Application.Current.Resources["FirstName"].ToString();
                string lastName = Application.Current.Resources["LastName"].ToString();
                nameTxtBlock.Text = String.Format("{0} {1}", firstName, lastName);

                //display airline information
                string airlineName = String.Empty;
                string airlineCode = Application.Current.Resources["AirlineCode"].ToString();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * " +
                        "FROM AirlineReference " +
                        "WHERE AirlineCode = '" + airlineCode + "'", con);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                airlineName = reader["AirlineName"].ToString();
                            }
                        }
                    }
                }
                airlineNameTxtBlock.Text = airlineName;
                
                logoImg.Source = getLogo(airlineCode);
            } catch (Exception ex){
                MessageBox.Show("Unexpected Error Occurred! Please contact Administator.", "Error");
                this.Close();
            }
        }

        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources.Clear();
            nameTxtBlock.Text = String.Empty;
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private static ImageSource getLogo (string airlineCode)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT AirlineLogoLarge FROM AirlineReference WHERE AirlineCode = '" + airlineCode + "'", con);
            object obj = cmd.ExecuteScalar();
            if(obj == null)
            {
                con.Close();
                return null;
            } else
            {
                byte[] bArray = (byte[])cmd.ExecuteScalar();
                con.Close();

                BitmapImage biImg = new BitmapImage();
                MemoryStream ms = new MemoryStream(bArray);
                biImg.BeginInit();
                biImg.StreamSource = ms;
                biImg.EndInit();

                ImageSource imgSrc = biImg as ImageSource;

                return imgSrc;
            }
        }
    }
}
