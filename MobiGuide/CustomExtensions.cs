using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CustomExtensions
{
    public static class StringExtension
    {
        public static string Shorten(this string str)
        {
            str = "..." + str.Substring(str.Length - 29, str.Length - (str.Length - 29));
            return str;
        }
    }

    public static class IntegerExtension
    {
        public static Color GetColor(this int i)
        {
            string hex = i.ToString("X6");
            Color color = (Color)ColorConverter.ConvertFromString("#" + hex);
            return color;
        }
    }

    public static class ColorExtension
    {
        public static int GetInteger(this Color color)
        {
            string colorString = string.Format("{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
            int i = int.Parse(colorString, System.Globalization.NumberStyles.HexNumber);
            return i;
        }
    }
}
