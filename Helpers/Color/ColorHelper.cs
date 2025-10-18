using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Color
{
    public static class ColorHelper
    {
        public static string DarkenHexColor(string hexColor, int percentage)
        {

            // Remove '#' if present
            hexColor = hexColor.TrimStart('#');

            // Convert hex to RGB
            int r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
            int g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
            int b = Convert.ToInt32(hexColor.Substring(4, 2), 16);

            // Convert RGB to HSL
            double h, s, l;
            RGBToHSL(r, g, b, out h, out s, out l);

            // Darken the lightness component
            l -= percentage / 100.0;
            l = Math.Max(0, l); // Ensure lightness is in the range [0, 1]

            // Convert HSL back to RGB
            HSLToRGB(h, s, l, out r, out g, out b);

            // Convert RGB to hex
            string darkHex = $"#{r:X2}{g:X2}{b:X2}";
            return darkHex;
        }

        public static void RGBToHSL(int r, int g, int b, out double h, out double s, out double l)
        {
            double dr = r / 255.0;
            double dg = g / 255.0;
            double db = b / 255.0;
            double max = Math.Max(dr, Math.Max(dg, db));
            double min = Math.Min(dr, Math.Min(dg, db));
            double delta = max - min;

            // Calculate lightness
            l = (max + min) / 2;

            // Calculate hue and saturation
            if (delta == 0)
            {
                h = 0; // undefined, achromatic
                s = 0;
            }
            else
            {
                s = l > 0.5 ? delta / (2 - max - min) : delta / (max + min);
                if (max == dr)
                    h = (dg - db) / delta + (dg < db ? 6 : 0);
                else if (max == dg)
                    h = (db - dr) / delta + 2;
                else
                    h = (dr - dg) / delta + 4;
                h /= 6;
            }
        }

        public static void HSLToRGB(double h, double s, double l, out int r, out int g, out int b)
        {
            if (s == 0)
            {
                r = g = b = (int)(l * 255);
            }
            else
            {
                double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                double p = 2 * l - q;
                r = HueToRGB(p, q, h + 1.0 / 3);
                g = HueToRGB(p, q, h);
                b = HueToRGB(p, q, h - 1.0 / 3);
            }
        }

        public static int HueToRGB(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6) return (int)((p + (q - p) * 6 * t) * 255);
            if (t < 1.0 / 2) return (int)(q * 255);
            if (t < 2.0 / 3) return (int)((p + (q - p) * (2.0 / 3 - t) * 6) * 255);
            return (int)(p * 255);
        }

    }
    public static class GraphColor {
		public static readonly string[] ColorScheme1 =
	{
		"#202740",
		"#EDE4DA",
		"#AAD2C8",
		"#3D516E"
	};

		public static readonly string[] ColorScheme2 =
		{
		"#E68369",
		"#ECCEAE",
		"#EDE4DA",
		"#202740",
		"#3D516E"
	};
	}
}
