using System;

namespace Project2048.Core
{
    using Board = UInt64;
    using Line = UInt16;
    public static class BitBoardHandler
    {
        public const int levelMask = 0xF;
        public const Board colMask = 0x000F000F000F000FUL;
        public const Board rowMask = 0xFFFFUL;
        public static Line GetLine(this Board board, int lineIndex)
        {
            switch (lineIndex)
            {
                case 0:
                    return (Line)(board & rowMask);
                case 1:
                    return (Line)((board >> 16) & rowMask);
                case 2:
                    return (Line)((board >> 32) & rowMask);
                case 3:
                    return (Line)((board >> 48) & rowMask);
            }
            throw new ArgumentException("lineIndex must be between 0 and 3");
        }
        public static int[] ToLevels(Line line)
        {
            return new[]
                {
                    (line >> 0)&levelMask,
                    (line >> 4)&levelMask,
                    (line >> 8)&levelMask,
                    (line >> 12)&levelMask,
                };
        }
        public static Line ToLine(int[] levels)
        {
            if (levels.Length != 4)
            {
                throw new ArgumentException();
            }
            return (Line)((levels[0] << 0) |
                    (levels[1] << 4) |
                    (levels[2] << 8) |
                    (levels[3] << 12));
        }
        public static Board ToTranspose(this Line line)
        {
            Board board = line;
            return (board | (board << 12) | (board << 24) | (board << 36)) & colMask;
        }
        public static Line ToReverse(this Line lineToReverse)
        {
            return (Line)((lineToReverse >> 12) | ((lineToReverse >> 4) & 0x00F0)
                | ((lineToReverse << 4) & 0x0F00) | (lineToReverse << 12));
        }
        public static Board ToTransposeLeft(this Board board)
        {
            Board a1 = board & 0xF0F00F0FF0F00F0FUL;
            Board a2 = board & 0x0000F0F00000F0F0UL;
            Board a3 = board & 0x0F0F00000F0F0000UL;
            Board a = a1 | (a2 << 12) | (a3 >> 12);
            Board b1 = a & 0xFF00FF0000FF00FFUL;
            Board b2 = a & 0x00FF00FF00000000UL;
            Board b3 = a & 0x00000000FF00FF00UL;
            return b1 | (b2 >> 24) | (b3 << 24);
        }
        public static Board ToTransposeRight(this Board board)
        {
            var a1 = board & 0x0F0FF0F00F0FF0F0UL;
            var a2 = board & 0x00000F0F00000F0FUL;
            var a3 = board & 0xF0F00000F0F00000UL;
            var a = a1 | (a2 << 20) | (a3 >> 20);
            var b1 = a & 0x00FF00FFFF00FF00UL;
            var b2 = a & 0xFF00FF0000000000UL;
            var b3 = a & 0x0000000000FF00FFUL;
            return b1 | (b2 >> 40) | (b3 << 40);
        }
        public static void Print(Board bitBoard)
        {
            for (int row = 0; row < 4; ++row)
            {
                for (int col = 0; col < 4; ++col)
                {
                    PrintBetweenCols();
                    int level = (int)(bitBoard & levelMask);
                    Console.Write("{0}", ToValue(level));
                    bitBoard >>= 4;
                }
                PrintBetweenRows();

            }
            PrintBetweenBoard();
        }

        public static int ToValue(int level)
        {
            return (level == 0) ? 0 : (1 << level);
        }

        private static void PrintBetweenCols()
        {
            Console.Write("\t");
        }
        private static void PrintBetweenRows()
        {
            Console.Write("\n");
            Console.Write("\n");
        }
        private static void PrintBetweenBoard()
        {
            Console.Write("\n");
        }

    }
}
