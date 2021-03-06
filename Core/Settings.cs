﻿namespace Project2048.Core
{
    public class Settings
    {
        public enum Direction
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
            None = 4,
        }

        public static readonly Direction[] Directions =
        {
            Direction.Up, Direction.Down, Direction.Left, Direction.Right,
        };

        public const int maxSearchMilliSecs = 100;

        public const int maxRound = 1;
        public const bool printProcess = true;
        public const bool onAnalyse = true;
        public const bool onEvolve = false;
    }
}
