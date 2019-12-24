using System.Windows.Controls;

namespace Minesweeper.View
{
    public class Cell : Button
    {
        public int R { get; }
        public int C { get; }

        public Cell(int r, int c, GameGrid gameGrid)
        {
            R = r;
            C = c;
            Click += gameGrid.Cell_Click;
            MouseRightButtonDown += gameGrid.Cell_MouseRightButtonDown;
        }
    }
}