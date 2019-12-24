using System;
using System.Collections;
using System.Collections.Generic;
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
                if (_game.IsVisited(c.R,c.C))
                {
                    e.Handled = true;
                    return;
                }

                _game.NextFlag(c.R, c.C);
                Invalidate(c);
            }

            e.Handled = true;
        }

        private readonly int[] _cx = {-1, 0, 1, 0};
        private readonly int[] _cy = {0, -1, 0, 1};

        public void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (!_game.GameRun)
            {
                e.Handled = true;
                return;
            }

            if (sender is Cell cell)
            {
                if (_game.GetFlag(cell.R, cell.C) != CellType.None)
                {
                    return;
                }

                bool b = _game.IsBomb(cell.R, cell.C);
                if (b)
                {
                    Clear(); //TODO 새로시작 테스트용
                    e.Handled = true;
                    return;
                }

                _game.SetVisited(cell.R, cell.C);
                if (_game.CalcVal(cell.R, cell.C) == 0)
                {
                    Bfs(cell);

                    Invalidate();
                }
                else
                {
                    Invalidate(cell);
                }
            }

            e.Handled = true;
        }

        private void Bfs(Cell cell)
        {
            var q = new Queue();
            q.Enqueue(new KeyValuePair<int, int>(cell.R, cell.C));

            while (q.Count != 0)
            {
                var t = (KeyValuePair<int, int>) q.Dequeue();

                foreach (var i in _cx)
                {
                    int r = t.Key + i;
                    if (r < 0 || r >= _game.Row)
                    {
                        continue;
                    }

                    foreach (var j in _cy)
                    {
                        int c = t.Value + j;

                        if (c < 0 || c >= _game.Column)
                        {
                            continue;
                        }

                        if (_game.IsVisited(r, c)) continue;

                        _game.SetVisited(r, c);
                        if (_game.CalcVal(r, c) == 0)
                        {
                            q.Enqueue(new KeyValuePair<int, int>(r, c));
                        }
                    }
                }
            }
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
            var type = _game.GetFlag(c.R, c.C);
            if (type != CellType.None)
            {
                switch (type)
                {
                    case CellType.Flag:
                        c.Background = new SolidColorBrush(Colors.Orange);
                        c.Content = MakeImagePanel(Properties.Resources.flag);
                        break;
                    case CellType.Question:
                        c.Background = new SolidColorBrush(Colors.YellowGreen);
                        c.Content = MakeImagePanel(Properties.Resources.question_mark);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (_game.IsVisited(c.R, c.C))
            {
                var val = _game.CalcVal(c.R, c.C);
                c.Content = val == 0 ? "" : val.ToString();
                c.Foreground = new SolidColorBrush(_numberColors[val]);
                c.Background = new SolidColorBrush(_backgroundColor);
                c.FontSize = FontSize;
                c.BorderThickness = new Thickness(2);
            }
            else
            {
                c.Background = new SolidColorBrush(Colors.CornflowerBlue);
                c.BorderThickness = new Thickness(1);
                c.Content = "";
            }
        }

        private static StackPanel MakeImagePanel(System.Drawing.Image resource, double margin = 10)
        {
            var bitmapStream = new MemoryStream();
            resource.Save(bitmapStream, System.Drawing.Imaging.ImageFormat.Png);
            bitmapStream.Seek(0, SeekOrigin.Begin);
            var newBitmapFrame = BitmapFrame.Create(bitmapStream);
            var image = new Image {Source = newBitmapFrame};
            var stackPnl = new StackPanel {Orientation = Orientation.Horizontal, Margin = new Thickness(margin)};
            stackPnl.Children.Add(image);
            return stackPnl;
        }
    }
}