using System;
using System.Collections.Generic;

namespace Project2048
{
    using Board = UInt64;
    using Direction = Settings.Direction;
    using Line = UInt16;
    public class ChessBoard
    {
        static ChessBoard()
        {
            InitializeMoveResults();
        }
        public ChessBoard() { }
        public ChessBoard(ChessBoard chessBoard)
        {
            BitBoard = chessBoard.BitBoard;
        }
        private ChessBoard(Board board)
        {
            BitBoard = board;
        }
        #region const and static
        public static readonly int[] AddLevels = { 1, 2 };
        public const double LevelTwoPossibility = 0.1;
        public const int LineMaxValue = 65536;

        private const int boardSize = 16;
        private static readonly Direction[] directions = Settings.Directions;
        private static readonly Position[] InBoardPositions =
        {
            new Position(0,0), new Position(0,1), new Position(0,2), new Position(0,3),
            new Position(1,0), new Position(1,1), new Position(1,2), new Position(1,3),
            new Position(2,0), new Position(2,1), new Position(2,2), new Position(2,3),
            new Position(3,0), new Position(3,1), new Position(3,2), new Position(3,3),
        };

        private const int LevelMask = 0xF;
        private static readonly Line[] moveLeftLines = new Line[LineMaxValue];
        private static readonly Line[] moveRightLines = new Line[LineMaxValue];
        private static readonly Board[] moveUpLines = new Board[LineMaxValue];
        private static readonly Board[] moveDownLines = new Board[LineMaxValue];
        private static readonly double[] lineScores = new double[LineMaxValue];
        #endregion


        public Board BitBoard { get; private set; } = 0;
        public int Score
        {
            get
            {
                return (int)GetLinesScoresOfTables(BitBoard, lineScores);
            }
        }
        public int EmptyCount
        {
            get
            {
                if (BitBoard == 0UL)
                {
                    return boardSize;
                }
                return GetEmptyCount();
            }
        }
        public int MaxValue
        {
            get
            {
                return GetMaxValue();
            }
        }
        public int DistinctCount
        {
            get
            {
                return GetDistinctValuesCount();
            }
        }
        public override bool Equals(object other)
        {
            if (other.GetType() != GetType() && other.GetType() != typeof(string))
            {
                return false;
            }
            return Equals((ChessBoard)other);
        }
        private bool Equals(ChessBoard other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(BitBoard, other.BitBoard);
        }
        public static void InitializeMoveResults()
        {
            for (int line = 0; line < LineMaxValue; ++line)
            {
                CacheLineScores(line);
                CacheLineMoveResult(line);
            }
        }
        private static void CacheLineScores(int line)
        {
            int[] levels = BitBoardHandler.ToLevels((Line)line);
            int score = 0;
            for (int i = 0; i < 4; ++i)
            {
                int level = levels[i];
                if (level >= 2)
                {
                    score += (level - 1) * (1 << level);
                }
            }
            lineScores[line] = score;
        }
        private static void CacheLineMoveResult(int line)
        {
            Line result = MoveLineLeft(line);

            Line reverseResult = result.ToReverse();
            Line reverseLine = ((Line)line).ToReverse();

            Board transposeLine = ((Line)line).ToTranspose();
            Board transposeResult = result.ToTranspose();
            Board transposeReverseLine = reverseLine.ToTranspose();
            Board transposeReverseResult = reverseResult.ToTranspose();

            moveLeftLines[line] = (Line)(line ^ result);
            moveRightLines[reverseLine] = (Line)(reverseLine ^ reverseResult);
            moveUpLines[line] = transposeLine ^ transposeResult;
            moveDownLines[reverseLine] = transposeReverseLine ^ transposeReverseResult;
        }
        private static Line MoveLineLeft(int line)
        {
            int[] levels = BitBoardHandler.ToLevels((Line)line);

            for (int i = 0; i < 3; ++i)
            {
                int j = i + 1;
                while (j < 4 && levels[j] == 0)
                {
                    ++j;
                }
                if (j >= 4)
                {
                    break;
                }
                if (levels[i] == 0)
                {
                    levels[i] = levels[j];
                    levels[j] = 0;
                    --i;
                }
                else if (levels[i] == levels[j])
                {
                    if (levels[i] != LevelMask)
                    {
                        ++levels[i];
                    }
                    levels[j] = 0;
                }
            }
            return BitBoardHandler.ToLine(levels);
        }
        private double GetLinesScoresOfTables(Board board, double[] scoreTables)
        {
            return scoreTables[board.GetLine(0)] +
                   scoreTables[board.GetLine(1)] +
                   scoreTables[board.GetLine(2)] +
                   scoreTables[board.GetLine(3)];
        }
        public Board MoveUp()
        {
            Board result = BitBoard;
            Board transResult = result.ToTransposeLeft();
            result ^= moveUpLines[transResult.GetLine(0)] << 0;
            result ^= moveUpLines[transResult.GetLine(1)] << 4;
            result ^= moveUpLines[transResult.GetLine(2)] << 8;
            result ^= moveUpLines[transResult.GetLine(3)] << 12;
            return result;
        }
        public Board MoveDown()
        {
            Board result = BitBoard;
            Board transResult = result.ToTransposeLeft();
            result ^= moveDownLines[transResult.GetLine(0)] << 0;
            result ^= moveDownLines[transResult.GetLine(1)] << 4;
            result ^= moveDownLines[transResult.GetLine(2)] << 8;
            result ^= moveDownLines[transResult.GetLine(3)] << 12;
            return result;
        }
        public Board MoveLeft()
        {
            Board result = BitBoard;
            result ^= (Board)moveLeftLines[BitBoard.GetLine(0)] << 0;
            result ^= (Board)moveLeftLines[BitBoard.GetLine(1)] << 16;
            result ^= (Board)moveLeftLines[BitBoard.GetLine(2)] << 32;
            result ^= (Board)moveLeftLines[BitBoard.GetLine(3)] << 48;
            return result;
        }
        public Board MoveRight()
        {
            Board result = BitBoard;
            result ^= (Board)moveRightLines[BitBoard.GetLine(0)] << 0;
            result ^= (Board)moveRightLines[BitBoard.GetLine(1)] << 16;
            result ^= (Board)moveRightLines[BitBoard.GetLine(2)] << 32;
            result ^= (Board)moveRightLines[BitBoard.GetLine(3)] << 48;
            return result;
        }

        private int GetEmptyCount()
        {
            var board = BitBoard;
            board |= (board >> 2) & 0x3333333333333333UL;
            board |= (board >> 1);
            board = ~board & 0x1111111111111111UL;

            board += board >> 32;
            board += board >> 16;
            board += board >> 8;
            board += board >> 4;
            return (int)board & LevelMask;
        }
        public void SetEmpty(Position position)
        {
            var emptyMask = (Board)LevelMask << position;
            emptyMask = ~emptyMask;
            BitBoard &= emptyMask;
        }
        public void RandomAdd()
        {
            if (EmptyCount > 0)
            {
                AddNew(RandomPosition(), RandomLevel());
            }
        }
        public void AddNew(Position position, int level)
        {
            BitBoard |= (Board)level << position;
        }
        private Position RandomPosition()
        {
            List<Position> emptyPositions = GetEmptyPositions();
            int randomIndex = RandomGenerator.Next(emptyPositions.Count);
            return emptyPositions[randomIndex];
        }
        /// <summary>
        /// 以0.9的概率获取2, 以0.1的概率获取4
        /// </summary>
        /// <returns></returns> 根据概率返回的level
        private int RandomLevel()
        {
            int number = RandomGenerator.Next(10);
            return number < LevelTwoPossibility * 10 ? AddLevels[1] : AddLevels[0];
        }
        public List<Position> GetEmptyPositions()
        {
            List<Position> EmptyPositions = new List<Position>();
            Board board = BitBoard;
            foreach (Position position in InBoardPositions)
            {
                if ((board & LevelMask) == 0)
                {
                    EmptyPositions.Add(position);
                }
                board >>= 4;
            }
            return EmptyPositions;
        }
        private int GetMaxValue()
        {
            Board board = BitBoard;
            int maxLevel = 0;
            while (board > 0)
            {
                int level = (int)(board & LevelMask);
                if (level > maxLevel)
                {
                    maxLevel = level;
                }
                board >>= 4;
            }
            return ToValue(maxLevel);
        }
        private int GetDistinctValuesCount()
        {
            HashSet<int> distinctLevels = new HashSet<int>();
            Board board = BitBoard;
            while (board > 0)
            {
                int level = (int)(board & LevelMask);
                distinctLevels.Add(level);
                board >>= 4;
            }
            return distinctLevels.Count;
        }
        private int ToValue(int level)
        {
            return BitBoardHandler.ToValue(level);
        }
        public bool Move(Direction direction)
        {
            Board result = GetMoveResult(direction);
            if (result != BitBoard)
            {
                BitBoard = result;
                return true;
            }
            return false;
        }

        private Board GetMoveResult(Direction direction)
        {
            Board result = BitBoard;
            switch (direction)
            {
                case Direction.Up:
                    result = MoveUp();
                    break;
                case Direction.Down:
                    result = MoveDown();
                    break;
                case Direction.Left:
                    result = MoveLeft();
                    break;
                case Direction.Right:
                    result = MoveRight();
                    break;
                default:
                    break;
            }
            return result;
        }

        public bool IsGameOver()
        {
            return EmptyCount <= 0 && !CanMove();
        }
        public bool CanMove()
        {
            foreach (Direction direction in directions)
            {
                if (CanMove(direction))
                {
                    return true;
                }
            }
            return false;
        }
        public bool CanMove(Direction direction)
        {
            Board result = GetMoveResult(direction);
            return result != BitBoard;
        }
        public ChessBoard Copy()
        {
            return new ChessBoard(this);
        }
        public void Print()
        {
            BitBoardHandler.Print(BitBoard);
        }
        public ChessBoard ToTransposeRight()
        {
            return new ChessBoard(BitBoard.ToTransposeRight());
        }
        public ChessBoard ToTransposeLeft()
        {
            return new ChessBoard(BitBoard.ToTransposeLeft());
        }
        public override int GetHashCode()
        {
            return BitBoard.GetHashCode();
        }
    }
}
