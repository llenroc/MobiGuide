using System.Windows;
using MobiGuide.Class;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for TextDisplayWindow.xaml
    /// </summary>
    public partial class TextDisplayWindow : Window
    {
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
                    Title = Messages.TITLE_NEW_TEXT_TRANSLATION;
                    break;
                case STATUS.EDIT:
                    Title = Messages.TITLE_EDIT_TEXT_TRANSLATION;
                    DisplayText = displayText;
                    break;
            }
        }

        private STATUS Status { get; }
        private string LanguageCode { get; }
        private string LanguageName { get; }
        public string DisplayText;

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
            Close();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
