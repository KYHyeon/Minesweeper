using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Minesweeper.Model;

namespace Minesweeper.View
{
    public class GameGrid : Grid
    {
        private Game _game;

        public GameGrid(int row, int column)
        {
            Init(row, column);
        }

        private void Init(int row, int column)
        {
            _game = new Game(row, column, row * column / 6, this);

            RowDefinitions.Clear();
            for (int i = 0; i < row; i++)
            {
                RowDefinitions.Add(new RowDefinition());
            }

            ColumnDefinitions.Clear();
            for (int i = 0; i < column; i++)
            {
                ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    var cell = new Cell(i, j, this);
                    SetRow(cell, i);
                    SetColumn(cell, j);
                    Children.Add(cell);
                }
            }

            Invalidate();
        }

        private void Clear()
        {
            Init(_game.Row, _game.Column);
        }

        public void Cell_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_game.GameRun)
            {
                e.Handled = true;
                return;
            }

            if (sender is Cell c)
            {
                _game.NextFlag(c.Row, c.Column);
                Invalidate(c);
            }

            e.Handled = true;
        }

        public void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (!_game.GameRun)
            {
                e.Handled = true;
                return;
            }

            if (sender is Cell c)
            {
                if (_game.GetFlag(c.Row, c.Column) != CellType.None)
                {
                    return;
                }

                bool b = _game.IsBomb(c.Row, c.Column);
//                c.Content = b.ToString();
                if (b)
                {
                    Clear(); //TODO 새로시작 테스트용
                }
                else
                {
                    _game.SetCheck(c.Row, c.Column);
//                    c.Content = _game.CalcVal(c.Row, c.Column);
                }

                Invalidate(c);
            }

            e.Handled = true;
        }

        public void Invalidate()
        {
            foreach (UIElement uiElement in Children)
            {
                if (uiElement is Cell c) Invalidate(c);
            }
        }

        private readonly Color[] _numberColors =
        {
            Colors.White, Colors.SkyBlue, Colors.Navy, Colors.MediumPurple, Colors.IndianRed, Colors.DarkSalmon,
            Colors.DarkCyan,
            Colors.Yellow, Colors.SaddleBrown
        };

        private readonly Color _backgroundColor = Colors.AliceBlue;
        public int FontSize { get; } = 20;

        public void Invalidate(Cell c)
        {
            CellType type = _game.GetFlag(c.Row, c.Column);
            if (type != CellType.None)
            {
                switch (type)
                {
                    case CellType.Flag:
                        c.Background = new SolidColorBrush(Colors.Orange);
                        Image image = new Image();
                        MemoryStream bitmapStream = new MemoryStream();
                        Properties.Resources.flag.Save(bitmapStream, System.Drawing.Imaging.ImageFormat.Png);
                        bitmapStream.Seek(0, SeekOrigin.Begin);
                        BitmapFrame newBitmapFrame = BitmapFrame.Create(bitmapStream);
                        image.Source = newBitmapFrame;

                        StackPanel stackPnl = new StackPanel();
                        stackPnl.Orientation = Orientation.Horizontal;
                        stackPnl.Margin = new Thickness(10);
                        stackPnl.Children.Add(image);
                        c.Content = stackPnl;
                        break;
                    case CellType.Question:
                        c.Background = new SolidColorBrush(Colors.YellowGreen);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (_game.IsChecked(c.Row, c.Column))
            {
                int val = _game.CalcVal(c.Row, c.Column);
                c.Content = val == 0 ? "" : val.ToString();
                c.Foreground = new SolidColorBrush(_numberColors[val]);
                c.Background = new SolidColorBrush(_backgroundColor);
                c.FontSize = FontSize;
                c.BorderThickness = new Thickness(3);
            }
            else
            {
                c.Background = new SolidColorBrush(Colors.CornflowerBlue);
                c.BorderThickness = new Thickness(1);
            }
        }
    }
}