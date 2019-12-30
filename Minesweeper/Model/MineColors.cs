using System.Windows.Media;

namespace Minesweeper.Model
{
    public class MineColors
    {
        public static Color[] NumberColors { get; } =
        {
            Colors.White, Colors.SkyBlue, Colors.Navy, Colors.MediumPurple, Colors.IndianRed, Colors.DarkSalmon,
            Colors.DarkCyan,
            Colors.Yellow, Colors.SaddleBrown
        };

        public static Color BackgroundColor { get; } = Colors.AliceBlue;
    }
}