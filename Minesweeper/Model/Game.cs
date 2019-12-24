﻿using System;
using Microsoft.Win32;
using Minesweeper.View;

namespace Minesweeper.Model
{
    public class Game
    {
        public int Row { get; }
        public int Column { get; }

        private CellType[,] _flagMap;
        private bool[,] _mineMap;
        private bool[,] _checkMap;
        private readonly int _bombCnt;
        private readonly Random _random = new Random();
        private GameGrid _gameGrid;

        public bool GameRun { get; private set; }

        public Game(int row, int column, int bombCnt, GameGrid gameGrid)
        {
            Row = row;
            Column = column;
            _bombCnt = bombCnt;
            _gameGrid = gameGrid;
            Clear();
        }

        public void Clear()
        {
            _flagMap = new CellType[Row, Column];
            _mineMap = new bool[Row, Column];
            _checkMap = new bool[Row, Column];
            int cnt = 0;
            while (cnt < _bombCnt)
            {
                int r = _random.Next(Row);
                int c = _random.Next(Column);
                if (_mineMap[r, c]) continue;
                _mineMap[r, c] = true;
                cnt++;
            }

            GameRun = true;
        }

        public int CalcVal(int r, int c)
        {
            int ret = 0;
            for (int i = -1; i <= 1; i++)
            {
                if (r + i < 0 || r + i >= Row)
                {
                    continue;
                }

                for (int j = -1; j <= 1; j++)
                {
                    if (c + j < 0 || c + j >= Column)
                    {
                        continue;
                    }

                    ret += _mineMap[r + i, c + j] ? 1 : 0;
                }
            }

            return ret;
        }

        public bool IsBomb(int r, int c)
        {
            return _mineMap[r, c];
        }

        public CellType NextFlag(int r, int c)
        {
            return _flagMap[r, c] = (CellType) ((int) ++_flagMap[r, c] % (int) CellType.Length);
        }

        public void GameEnd()
        {
            GameRun = false;
        }

        private bool IsValid(int r, int c)
        {
            return r >= 0 && r < Row && c >= 0 && c < Column;
        }

        public bool IsChecked(int r, int c)
        {
            return _checkMap[r, c];
        }

        public void SetCheck(int r, int c)
        {
            _checkMap[r, c] = true;
        }

        public CellType GetFlag(int r, int c)
        {
            return _flagMap[r, c];
        }
    }
}