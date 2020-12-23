using Emgu.CV;
using Emgu.CV.Structure;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ThermoCam160B
{
    public static class Palette
    {
        private static int maxLookUpTable = 256;

        private static OxyPalette WhiteHot(int numberOfColors)
        {
            return OxyPalette.Interpolate(
                numberOfColors,
                OxyColors.Black,
                OxyColor.FromRgb(28, 28, 28),
                OxyColor.FromRgb(63, 63, 63),
                OxyColor.FromRgb(93, 93, 93),
                OxyColor.FromRgb(125, 125, 125),
                OxyColor.FromRgb(160, 160, 160),
                OxyColor.FromRgb(192, 192, 192),
                OxyColor.FromRgb(222, 222, 222),
                OxyColors.White
                );
        }

        private static OxyPalette ColdHot(int numberOfColors)
        {
            return OxyPalette.Interpolate(
                numberOfColors,
                OxyColors.Blue,
                OxyColor.FromRgb(28, 28, 28),
                OxyColor.FromRgb(63, 63, 63),
                OxyColor.FromRgb(93, 93, 93),
                OxyColor.FromRgb(125, 125, 125),
                OxyColor.FromRgb(160, 160, 160),
                OxyColor.FromRgb(192, 192, 192),
                OxyColor.FromRgb(222, 222, 222),
                OxyColors.Red
                );
        }

        private static OxyPalette BlackHot(int numberOfColors)
        {
            return OxyPalette.Interpolate(
                numberOfColors,
                OxyColors.White,
                OxyColor.FromRgb(222, 222, 222),
                OxyColor.FromRgb(192, 192, 192),
                OxyColor.FromRgb(160, 160, 160),
                OxyColor.FromRgb(125, 125, 125),
                OxyColor.FromRgb(93, 93, 93),
                OxyColor.FromRgb(63, 63, 63),
                OxyColor.FromRgb(28, 28, 28),
                OxyColors.Black
                );
        }

        private static OxyPalette HotSpot(int numberOfColors)
        {
            // FIXME
            return OxyPalette.Interpolate(
                numberOfColors,
                OxyColor.FromRgb(0, 0, 0),
                OxyColor.FromRgb(24, 27, 24),
                OxyColor.FromRgb(58, 60, 57),
                OxyColor.FromRgb(90, 92, 89),
                OxyColor.FromRgb(121, 124, 121),
                OxyColor.FromRgb(158, 162, 161),
                OxyColor.FromRgb(248, 97, 0),
                OxyColor.FromRgb(226, 32, 0),
                OxyColor.FromRgb(160, 19, 0)
                );
        }

        private static OxyPalette ColdSpot(int numberOfColors)
        {
            // FIXME
            return OxyPalette.Interpolate(
                numberOfColors,
                OxyColor.FromRgb(2, 33, 183),
                OxyColor.FromRgb(2, 46, 201),
                OxyColor.FromRgb(0, 158, 248),
                OxyColor.FromRgb(161, 161, 160),
                OxyColor.FromRgb(120, 124, 120),
                OxyColor.FromRgb(89, 91, 88),
                OxyColor.FromRgb(64, 65, 63),
                OxyColor.FromRgb(32, 34, 31),
                OxyColors.Black
                );
        }

        private static OxyPalette Rainbow(int numberOfColors)
        {
            return OxyPalette.Interpolate(
                numberOfColors,
                OxyColors.Blue,
                OxyColors.Cyan,
                OxyColors.LimeGreen,
                OxyColors.Yellow,
                OxyColors.Red
                );
        }

        private static OxyPalette Ironbow(int numberOfColors)
        {
            return OxyPalette.Interpolate(
                numberOfColors,
                OxyColors.Black,
                OxyColor.FromRgb(46, 5, 128),
                OxyColor.FromRgb(120, 0, 153),
                OxyColor.FromRgb(194, 19, 134),
                OxyColor.FromRgb(226, 70, 28),
                OxyColor.FromRgb(240, 126, 5),
                OxyColor.FromRgb(250, 184, 8),
                OxyColor.FromRgb(249, 226, 69),
                OxyColors.White
                );
        }

        static public Mat GetPresetColorMat(string strPalette)
        {
            OxyPlot.OxyPalette palette = null;
            Mat colorMap = new Mat(maxLookUpTable, 1, Emgu.CV.CvEnum.DepthType.Cv8U, 3);
            Image<Bgr, byte> colorImage = new Image<Bgr, byte>(1, maxLookUpTable);
            
            switch (strPalette)
            {
                case "WhiteHot":
                    palette = Palette.WhiteHot(maxLookUpTable);
                    break;

                case "BlackHot":
                    palette = Palette.BlackHot(maxLookUpTable);
                    break;

                case "ColdHot":
                    palette = Palette.ColdHot(maxLookUpTable);
                    break;

                case "HotSpot":
                    palette = Palette.HotSpot(maxLookUpTable);
                    break;

                case "ColdSpot":
                    palette = Palette.ColdSpot(maxLookUpTable);
                    break;

                case "Rainbow":
                    palette = Palette.Rainbow(maxLookUpTable);
                    break;

                case "Ironbow":
                    palette = Palette.Ironbow(maxLookUpTable);
                    break;

                case "Cool":
                    palette = OxyPlot.OxyPalettes.Cool(maxLookUpTable);
                    break;

                case "Hot":
                    palette = OxyPlot.OxyPalettes.Hot(maxLookUpTable);
                    break;

                case "Gray":
                    palette = OxyPlot.OxyPalettes.Gray(maxLookUpTable);
                    break;

                case "Hue":
                    palette = OxyPlot.OxyPalettes.Hue(maxLookUpTable);
                    break;

                case "Jet":
                    palette = OxyPlot.OxyPalettes.Jet(maxLookUpTable);
                    break;

                case "Transparent":
                    palette = OxyPalette.Interpolate(maxLookUpTable, OxyColors.Transparent);
                    break;

                default:
                case "None":
                    //palette = null; //
                    palette = OxyPlot.OxyPalettes.Gray(maxLookUpTable);   // Default "None" is gray
                    break;
            }

            if(palette != null)
            {
                OxyColor[] colorTable = palette.Colors.ToArray();

                IntPtr ptrData = colorMap.DataPointer;
                for (var index = 0; index < palette.Colors.Count; index++)
                {
                    byte[] rgb = new byte[3] { colorTable[index].B, colorTable[index].G, colorTable[index].R };
                    Marshal.Copy(rgb, 0, ptrData + index * rgb.Length, rgb.Length);
                }
            }

            return colorMap;
        }

        static public void ApplyColorMap(ref Image<Bgr, byte> image, Mat palette)
        {
            if (image == null) return;

            CvInvoke.LUT(image, palette, image);
        }
    }
}
