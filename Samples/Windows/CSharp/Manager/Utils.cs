using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


#region EmguCV
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.Tiff;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
#endregion

namespace ThermoCam160B
{
    static public class Utils
    {
        public const string STR_SYMBOL_RAW = "";
        public const string STR_SYMBOL_CELSIUS = "℃";
        public const string STR_SYMBOL_FAHRENHEIT = "℉";
        public const string STR_SYMBOL_KELVIN = "K";

        public const double CONST_KELVIN2CELSIUS = 273.15;
        public const double CONST_KELVIN2FEHRENHEIT = 459.67;

        [DllImport( "kernel32.dll", CharSet = CharSet.Auto )]
        public static extern uint GetShortPathName( string lpszLongPath, StringBuilder lpszShortPath, uint cchBuffer );

        public static Image<Bgr, byte> ResizeImage(Image<Bgr, byte> srcImg, int width, int height, Emgu.CV.CvEnum.Inter interpolationType)
        {
            if (width == 0 || height == 0)
            {
                return null;
            }

            Image<Bgr, byte> imgScale = new Image<Bgr, byte>(width, height);
            CvInvoke.Resize(srcImg, imgScale, new System.Drawing.Size(width, height), 0, 0, interpolationType);
            return imgScale;
        }

        public static double ConvertRawToCelsius(double raw)
        {
            return raw / 100 - CONST_KELVIN2CELSIUS;
        }

        public static double ConvertKelvinToCelsius(double kelvin)
        {
            return kelvin - CONST_KELVIN2CELSIUS;
        }

        public static double ConvertKelvinToFahrenheit(double kelvin)
        {
            return kelvin * 9 / 5 - CONST_KELVIN2FEHRENHEIT;
        }

        public static double CompensateHumanTemperature(double tempC)
        {
            double compTempC = tempC;
            if(25.0 < tempC && tempC <= 37.5)
            {
                compTempC = tempC - (0.9 * (tempC - 36.5));
            }else
            if( 37.5 < tempC && tempC <= 40.0)
            {
                compTempC = tempC - (0.9 * (tempC - 37.3));
            }
            return compTempC;
        }
    }
}
