using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Dashboard.Common.Card
{
    public class DashboardCardViewModel
    {
        public DashboardCardViewModel()
        {

        }
        public DashboardCardViewModel(string id, string title, string url)
        {
            Id = id;
            Title = title;
            Url = url;
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
    public class DashboardCardDataViewModel
    {
        public DashboardCardDataViewModel()
        {

        }
        public DashboardCardDataViewModel(string icon)
        {
            Icon = icon;
        }
        public string Icon { get; set; }
        public List<CardViewModel> Cards { get; set; } = new List<CardViewModel>();
    }

    public class CardViewModel
    {
        public CardViewModel()
        {

        }
        public CardViewModel(string title, string color)
        {
            Title = title;
            Color = color;

        }
        public CardViewModel(string title, string color,string icon)
        {
            Title = title;
            Color = color;
            Icon = icon;

        }
        public CardViewModel(string title, string value,string icon, string color)
        {
            Title = title;
            Value = value;
            Color = color;
            Icon = icon;
        }
        public CardViewModel(string title, string value,string icon, string color, string className)
        {
            Title = title;
            Value = value;
            Color = color;
            ClassName = className;
            Icon = icon;
        }

        public string Title { get; set; }
        public string Value { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public string BackgroundColor
        {
            get
            {
                return GetLighterShade(Color);
            }
        }

        public string ClassName { get; set; } = "col-12";

        private string GetLighterShade(string hexColor, int lightenStep = 100)
        {
            // Remove "#" symbol if present
            hexColor = hexColor.TrimStart('#');

            // Convert each hex digit to integer (0-255)
            int red = Convert.ToInt32(hexColor.Substring(0, 2), 16);
            int green = Convert.ToInt32(hexColor.Substring(2, 2), 16);
            int blue = Convert.ToInt32(hexColor.Substring(4, 2), 16);

            // Lighten each channel
            int newRed = Math.Min(255, red + lightenStep);
            int newGreen = Math.Min(255, green + lightenStep);
            int newBlue = Math.Min(255, blue + lightenStep);

            // Convert back to hex code string with leading zeros
            return $"#{newRed:X2}{newGreen:X2}{newBlue:X2}";
        }
    }
}
