using System.Windows;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for TextDisplayWindow.xaml
    /// </summary>
    public partial class TextDisplayWindow : Window
    {
        private STATUS Status { get; set; }
        private string LanguageCode { get; set; }
        private string LanguageName { get; set; }
        public string DisplayText { get; set; }
        public TextDisplayWindow(string languageCode, string languageName) : this(languageCode, languageName, null, STATUS.NEW) { }
        public TextDisplayWindow(string languageCode, string languageName, string displayText) : this(languageCode, languageName, displayText, STATUS.EDIT) { }
        public TextDisplayWindow(string languageCode, string languageName, string displayText, STATUS status)
        {
            InitializeComponent();

            Status = status;
            LanguageCode = languageCode;
            LanguageName = languageName;

            switch (Status)
            {
                case STATUS.NEW:
                    this.Title = "New Text Translation";
                    break;
                case STATUS.EDIT:
                    this.Title = "Edit Text Translation";
                    this.DisplayText = displayText;
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            languageCodeTextBox.Text = LanguageCode;
            languageNameTextBox.Text = LanguageName;
            displayTextTextBox.Text = DisplayText;
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            DisplayText = displayTextTextBox.Text;
            DialogResult = true;
            this.Close();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
