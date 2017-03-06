using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CustomExtensions
{
    public static class StringExtension
    {
        public static string Shorten(this string str)
        {
            str = "..." + str.Substring(str.Length - 29, str.Length - (str.Length - 29));
            return str;
        }
        public static string Shorten(this string str, int length)
        {
            return str.Length > length ? str.Substring(0, length - 4) + "..." : str;
        }

        public static bool IsNull(this string str)
        {
            return String.IsNullOrWhiteSpace(str);
        }
    }

    public static class IntegerExtension
    {
        public static Color GetColor(this int i)
        {
            string hex = i.ToString("X8");
            Color color = (Color)ColorConverter.ConvertFromString("#" + hex);
            return color;
        }
    }

    public static class ColorExtension
    {
        public static int GetInteger(this Color color)
        {
            string colorString = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
            int i = int.Parse(colorString, System.Globalization.NumberStyles.HexNumber);
            return i;
        }
    }

    public static class ImageSourceExtension
    {
        public static ImageSource BlobToSource(this object obj)
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
            catch (Exception)
            {
                return null;
            }
        }
    }

    public static class WindowExtension
    {
        public static void MaximizeToSecondaryMonitor(this Window window)
        {
            var secondaryScreen = System.Windows.Forms.Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

            if (secondaryScreen != null)
            {
                if (!window.IsLoaded)
                    window.WindowStartupLocation = WindowStartupLocation.Manual;

                var workingArea = secondaryScreen.WorkingArea;
                window.Left = workingArea.Left;
                window.Top = workingArea.Top;
                window.Width = workingArea.Width;
                window.Height = workingArea.Height;
                // If window isn't loaded then maxmizing will result in the window displaying on the primary monitor
                if (window.IsLoaded)
                    window.WindowState = WindowState.Maximized;
            }
        }
    }
}
