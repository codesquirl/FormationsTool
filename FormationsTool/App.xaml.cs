using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FormationsTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Color PlayerColor = Color.FromRgb(16, 32, 48);
        public static List<Color> PositionColors = new List<Color>()
        {
            Colors.DarkGreen,
            Colors.CornflowerBlue,
            Colors.Crimson,
            Colors.DarkOrange,
            Colors.MediumVioletRed,
            Colors.MediumSlateBlue,
            Colors.Gold,
            Colors.DarkGray
        };
    }
}
