using Project2048;
using System;

namespace Project2048MSTest
{
    public class ChessBoardHandler
    {

        public static int RandomLevel()
        {
            return RandomGenerator.Next(1, 16);
        }
        public static int[] GetRandomDistinctLevels(int count)
        {
            return RandomGenerator.GetDistinctInts(count, 1, 16);
        }
        public static void RandomCountAddLevelOne(ChessBoard chessBoard)
        {
            int occupyCount = RandomGenerator.Next(chessBoard.EmptyCount);
            RandomAddLevelOne(chessBoard, occupyCount);
        }

        public static void RandomAddLevelOne(ChessBoard chessBoard, int count)
        {
            var emptyPositions = chessBoard.CalculateAndGetEmptyPositions();
            int occupyCount = Math.Min(count, chessBoard.EmptyCount);
            var levels = RandomGenerator.GetDistinctInts(occupyCount, 1, 16);
            for (int i = 0; i < occupyCount; ++i)
            {
                chessBoard.AddNew(emptyPositions[i], levels[i]);
            }
        }
        public static void AddLeftDiagonal(ChessBoard chessBoard)
        {
            var levels = RandomGenerator.GetDistinctInts(4, 1, 16);
            chessBoard.AddNew(new Position(0, 0), levels[0]);
            chessBoard.AddNew(new Position(1, 1), levels[1]);
            chessBoard.AddNew(new Position(2, 2), levels[2]);
            chessBoard.AddNew(new Position(3, 3), levels[3]);
        }
        public static void AddSymmetryToLeftDiagonal(ChessBoard chessBoard)
        {
            var oneSideCount = RandomGenerator.Next(1, 6);
            var levels = RandomGenerator.GetDistinctInts(oneSideCount, 1, 16);
            var emptyPositions = chessBoard.CalculateAndGetEmptyPositions();
            int occupyCount = Math.Min(oneSideCount, chessBoard.EmptyCount / 2);
            for (int i = 0; i < occupyCount; ++i)
            {
                var position = emptyPositions[i];
                chessBoard.AddNew(position, levels[i]);
                chessBoard.AddNew(new Position(position.Col, position.Row), levels[i]);
            }
        }
        public static void AddRightDiagonal(ChessBoard chessBoard)
        {
            var levels = RandomGenerator.GetDistinctInts(4, 1, 16);
            chessBoard.AddNew(new Position(0, 3), levels[0]);
            chessBoard.AddNew(new Position(1, 2), levels[1]);
            chessBoard.AddNew(new Position(2, 1), levels[2]);
            chessBoard.AddNew(new Position(3, 0), levels[3]);
        }
        public static void AddSymmetryToRightDiagonal(ChessBoard chessBoard)
        {
            var oneSideCount = RandomGenerator.Next(1, 6);
            var levels = RandomGenerator.GetDistinctInts(oneSideCount, 1, 16);
            var emptyPositions = chessBoard.CalculateAndGetEmptyPositions();
            int occupyCount = Math.Min(oneSideCount, chessBoard.EmptyCount / 2);
            for (int i = 0; i < occupyCount; ++i)
            {
                var position = emptyPositions[i];
                chessBoard.AddNew(position, levels[i]);
                chessBoard.AddNew(new Position(3 - position.Col, 3 - position.Row), levels[i]);
            }
        }

    }
}
