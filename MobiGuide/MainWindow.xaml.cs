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

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    class UserInfo : INotifyPropertyChanged
    {
        // property changed event
        public event PropertyChangedEventHandler PropertyChanged;

        private string fullName;
        private string airlineName;
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public string FullName
        {
            get {
                return fullName;
            }
            set {
                fullName = value;
                OnPropertyChanged("FullName");
            }
        }
        public string AirlineName
        {
            get
            {
                return airlineName;
            }
            set
            {
                airlineName = value;
                OnPropertyChanged("AirlineName");
            }
        }
    }
    public partial class MainWindow : Window
    {
        static string connectionString = String.Empty;
        UserInfo userInfo = new UserInfo();
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
                //nameTxtBlock.Text = String.Format("{0} {1}", firstName, lastName);
                userInfo.FullName = String.Format("{0} {1}", firstName, lastName);

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
                Application.Current.Resources["AirlineName"] = airlineName;
                userInfo.AirlineName = airlineName;
                this.DataContext = userInfo;
                //display airline logo 
                ImageSource logoSource = getLogo(airlineCode);
                if(logoSource != null)
                {
                    logoImg.Source = logoSource;
                }
            } catch (Exception ex){
                MessageBox.Show("Unexpected Error Occurred! Please contact Administator.", "Error");
                this.Close();
            }
        }

        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Do you want to logout?", "Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //clear everything and do logout
                Application.Current.Resources.Clear();
                nameTxtBlock.Text = String.Empty;
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private static ImageSource getLogo (string airlineCode)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT AirlineLogoLarge FROM AirlineReference WHERE AirlineCode = '" + airlineCode + "'", con);
                    object obj = cmd.ExecuteScalar();
                    if (obj == DBNull.Value)
                    {
                        con.Close();
                        return null;
                    }
                    else
                    {
                        byte[] bArray = (byte[])obj;
                        con.Close();

                        BitmapImage biImg = new BitmapImage();
                        MemoryStream ms = new MemoryStream(bArray);
                        biImg.BeginInit();
                        biImg.StreamSource = ms;
                        biImg.EndInit();

                        ImageSource imgSrc = biImg as ImageSource;

                        return imgSrc;
                    }
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR");
                    return null;
                }
            }
        }

        private void editProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            EditProfileWindow editProfileWindow = new EditProfileWindow();
            editProfileWindow.ShowDialog();
        }

        private void addUserBtn_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addUserWindow = new AddUserWindow();
            addUserWindow.ShowDialog();
        }

        private void editALRefBtn_Click(object sender, RoutedEventArgs e)
        {
            EditAirlineReferenceWindow editALRefWindow = new EditAirlineReferenceWindow();
            editALRefWindow.ShowDialog();
        }
    }
}
