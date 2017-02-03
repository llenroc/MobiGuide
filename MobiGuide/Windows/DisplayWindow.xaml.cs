﻿using System;
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
using Properties;

namespace MobiGuide
{
    /// <summary>
    /// Interaction logic for DisplayWindow.xaml
    /// </summary>
    public partial class DisplayWindow : Window
    {
        private DISPLAY_TYPE DisplayType { get; set; }
        public DisplayWindow()
        {
            InitializeComponent();
        }
        public DisplayWindow(DISPLAY_TYPE type) : this()
        {
            this.DisplayType = type;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (DisplayType)
            {
                case DISPLAY_TYPE.LOGO:
                    DisplayFrame.Navigate(new LogoPage());
                    break;
            }
        }
    }
}