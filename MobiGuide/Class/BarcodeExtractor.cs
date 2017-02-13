using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiGuide
{
    class BarcodeExtractor
    {
        public string Barcode { get; set; }
        public BarcodeExtractor() { }
        public BarcodeExtractor(string Barcode)
        {
            this.Barcode = Barcode;
        }

        public string GetSeatNumber()
        {
            try
            {
                return GetSeatRow() + GetSeatColumn();
            } catch (ArgumentNullException)
            {
                return "";
            }
        }

        public string GetSeatColumn()
        {
            try
            {
                string seatNumber = Barcode.Substring(48, 4);
                return seatNumber.Substring(seatNumber.Length - 1, 1);
            }
            catch (ArgumentNullException)
            {
                return "";
            }
        }

        public string GetSeatRow()
        {
            try
            {
                string seatNumber = Barcode.Substring(48, 4);
                return Int32.Parse(seatNumber.Substring(0, seatNumber.Length - 1)).ToString();
            }
            catch (ArgumentNullException)
            {
                return "";
            }
        }
    }
}
