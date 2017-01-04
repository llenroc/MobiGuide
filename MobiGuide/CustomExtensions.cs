using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
