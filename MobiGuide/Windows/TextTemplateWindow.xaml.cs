using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DatabaseConnector;
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for TextTemplateWindow.xaml
    /// </summary>
    public partial class TextTemplateWindow : Window
    {
        DBConnector dbCon = new DBConnector();
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        public TextTemplateWindow()
        {
            InitializeComponent();

            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Any", Value = "" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Active", Value = "A" });
            statusComboBox.Items.Add(new CustomComboBoxItem { Text = "Inactive", Value = "I" });
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }
        private async void DoSearch()
        {
            string textName = textNameTextBox.Text;
            string textTemplate = textTemplateTextBox.Text;
            string status = statusComboBox.SelectedValue.ToString();

            DataList list = await dbCon.GetDataList("TextTemplate", null,
                String.Format("WHERE TextName LIKE '%{0}%' AND TextTemplate LIKE '%{1}%' AND StatusCode LIKE '%{2}%' COLLATE SQL_Latin1_General_CP1_CI_AS ORDER BY TextName",
                textName, textTemplate, status));
            if (list.HasData && list.Error == ERROR.NoError)
            {
                List<TextTemplate> _itemSource = new List<TextTemplate>();
                foreach (DataRow row in list)
                {
                    TextTemplate textTemplateRef = new TextTemplate
                    { 
                        TextName = row.Get("TextName").ToString(),
                        TextDisplay = row.Get("TextTemplate").ToString(),
                        Status = row.Get("StatusCode").ToString(),
                        RotateInSeconds = (int)row.Get("RotateInSeconds"),
                        TextTemplateId = (Guid)row.Get("TextTemplateId")
                    };
                    _itemSource.Add(textTemplateRef);
                }
                textTemplateDataGrid.ItemsSource = _itemSource;
            }
            else
            {
                textTemplateDataGrid.ItemsSource = null;
                textTemplateDataGrid.SelectedIndex = -1;
            }
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            NewEditTextTemplateWindow newEditTextTemplateWindow = new NewEditTextTemplateWindow();
            newEditTextTemplateWindow.ShowDialog();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            int indexOfSelectedItem = textTemplateDataGrid.SelectedIndex;
            if (indexOfSelectedItem > -1)
            {
                Guid textTemplateId = (textTemplateDataGrid.SelectedItem as TextTemplate).TextTemplateId;
                NewEditTextTemplateWindow editTextTemplateWindow = new NewEditTextTemplateWindow(textTemplateId);
                editTextTemplateWindow.ShowDialog();
                if (editTextTemplateWindow.DialogResult.HasValue && editTextTemplateWindow.DialogResult.Value)
                {
                    DoSearch();
                }
            }
        }

        private void textTemplateDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedIndex != -1) editBtn.IsEnabled = true;
            else editBtn.IsEnabled = false;
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            textNameTextBox.Text = String.Empty;
            textTemplateTextBox.Text = String.Empty;
            statusComboBox.SelectedIndex = 0;
            textTemplateDataGrid.ItemsSource = null;
            textTemplateDataGrid.SelectedIndex = -1;
        }

        private void textTemplateDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int indexOfSelectedItem = (sender as DataGrid).SelectedIndex;
            if(indexOfSelectedItem > -1)
            {
                Guid textTemplateId = (textTemplateDataGrid.SelectedItem as TextTemplate).TextTemplateId;
                NewEditTextTemplateWindow editTextTemplateWindow = new NewEditTextTemplateWindow(textTemplateId);
                editTextTemplateWindow.ShowDialog();
                if (editTextTemplateWindow.DialogResult.HasValue && editTextTemplateWindow.DialogResult.Value)
                {
                    DoSearch();
                }
            }
        }
    }
}
