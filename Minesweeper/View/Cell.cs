using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Minesweeper.Model;

namespace Minesweeper.View
{
    public class Cell : Button
    {
        public int Row { get; }
        public int Column { get; }

        public Cell(int row, int column, GameGrid gameGrid)
        {
            Row = row;
            Column = column;
            Click += gameGrid.Cell_Click;
            MouseRightButtonDown += gameGrid.Cell_MouseRightButtonDown;
        }

    }
}