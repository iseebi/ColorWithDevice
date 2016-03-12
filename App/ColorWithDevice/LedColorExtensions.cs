using ColorWithDevice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ColorWithDevice
{
    public static class LedColorExtensions
    {
        public static Color ToFormColor(this LedColor color)
        {
            switch (color)
            {
                case LedColor.Blue:
                    return Color.Blue;
                case LedColor.Green:
                    return Color.Green;
                case LedColor.Cyan:
                    return Color.Teal;
                case LedColor.Red:
                    return Color.Red;
                case LedColor.Magenta:
                    return Color.Fuchsia;
                case LedColor.Yellow:
                    return Color.Yellow;
                case LedColor.White:
                    return Color.White;
                default:
                    return Color.Black;
            }
        }
    }
}
