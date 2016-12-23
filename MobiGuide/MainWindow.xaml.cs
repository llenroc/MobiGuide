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

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
                string firstName = Application.Current.Resources["firstName"].ToString();
                string lastName = Application.Current.Resources["lastName"].ToString();
                nameTxtBlock.Text = String.Format("{0} {1}", firstName, lastName);
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
    }
}
