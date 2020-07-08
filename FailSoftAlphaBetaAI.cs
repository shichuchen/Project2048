using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Project2048
{
    using Direction = Settings.Direction;
    using Board = UInt64;
    public class FailSoftAlphaBetaAI : IPlayer
    {
        public class Decision
        {
            public Direction bestDirection = Direction.None;
            public Position bestPosition;
        }
        public class HashItem
        {
            public int depth = 0;
            public int hashFlag = -1;
            public double value = 0;
            public Decision decision = new Decision();

        }
        public FailSoftAlphaBetaAI(ChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }

        private readonly ChessBoard chessBoard;
        private static readonly Direction[] directions = Settings.Directions;
        private static readonly bool printProcess = Settings.PrintProcess;
        private static readonly int[] addLevels = ChessBoard.AddLevels;
        private const double infinity = Evaluator.Infinity;
        private const double bound = infinity / 2;
        private const double lostPenality = -infinity / 3;
        private const int maxMilliSecs = Settings.MaxSearchMilliSecs;
        private const int hashExact = 0;
        private const int hashAlpha = 1;
        private const int hashBeta = 2;
        private static Dictionary<Board, HashItem> hashTable;
        private static int targetDepth;
        private static int depth = 1;
        private static int curOff = 0;
        private Direction bestDirection = Direction.None;
        private static TimeRecorder timeRecorder;


        public Direction GetMoveDirection()
        {
            Initialize();
            targetDepth = Math.Max(6, (chessBoard.DistinctCount - 2) * 2);
            while (NotComplete())
            {
                MoveSearch(depth, -bound, bound);
                ++depth;
            }
            Analyser.StoreDepth(depth);
            Analyser.StoreCutOff(curOff);
            PrintProcess();
            return bestDirection;
        }

        private static void PrintProcess()
        {
            if (printProcess)
            {
                Console.WriteLine("\tDepth:\t{0}", depth);
                Console.WriteLine("\tCutOff:\t{0}\n", curOff);
            }
        }

        private static bool NotComplete()
        {
            return timeRecorder.GetTotalMilliSeconds() <= maxMilliSecs
                            || depth <= targetDepth;
        }

        private static void Initialize()
        {
            curOff = 0;
            depth = 1;
            hashTable = new Dictionary<Board, HashItem>();
            timeRecorder = new TimeRecorder();
        }

        private double ProbeHash(int depth, double alpha, double beta, out Decision prevDecision)
        {
            if (hashTable.TryGetValue(chessBoard.BitBoard, out var hashItem))
            {
                if (hashItem.depth >= depth)
                {
                    prevDecision = hashItem.decision;
                    switch (hashItem.hashFlag)
                    {
                        case hashExact:
                            return hashItem.value;
                        case hashAlpha:
                            if (hashItem.value <= alpha)
                            {
                                return alpha;
                            }
                            break;
                        case hashBeta:
                            if (hashItem.value >= beta)
                            {
                                return beta;
                            }
                            break;
                    }
                }
            }
            prevDecision = new Decision();
            return infinity;
        }
        private void RecordHash(int depth, double value, int hashFlag, Decision decision)
        {
            Board board = chessBoard.BitBoard;
            if (hashTable.TryGetValue(board, out HashItem hashItem))
            {
                if (hashItem.depth > depth)
                {
                    return;
                }

            }
            hashTable[board] = new HashItem()
            {
                value = value,
                hashFlag = hashFlag,
                depth = depth,
                decision = decision,
            };
        }
        public double MoveSearch(int depth, double alpha, double beta)
        {
            int hashFlag = hashAlpha;
            double val;
            if ((val = ProbeHash(depth, alpha, beta, out Decision prevDecision)) != infinity)
            {
                ++curOff;
                return val;
            }
            if (chessBoard.IsGameOver())
            {
                return lostPenality + depth;
            }
            if (depth == 0)
            {
                val = Evaluator.EvalForMove(chessBoard);
                RecordHash(depth, val, hashExact, prevDecision);
                return val;
            }
            Direction[] moveDirections = SortedDirections(prevDecision);
            foreach (Direction direction in moveDirections)
            {
                var newBoard = chessBoard.Copy();
                if (newBoard.Move(direction))
                {
                    var ai = new FailSoftAlphaBetaAI(newBoard);
                    val = -ai.AddSearch(depth - 1, -beta, -alpha);
                    if (val >= beta)
                    {
                        RecordHash(depth, beta, hashBeta, prevDecision);
                        return beta;
                    }
                    if (val > alpha)
                    {
                        hashFlag = hashExact;
                        alpha = val;
                        bestDirection = direction;
                        prevDecision.bestDirection = direction;
                    }
                }
            }
            RecordHash(depth, alpha, hashFlag, prevDecision);
            return alpha;
        }

        private static Direction[] SortedDirections(Decision prevDecision)
        {
            var prevBestDirection = prevDecision.bestDirection;
            if (prevBestDirection == Direction.None)
            {
                return directions;
            }
            Direction[] moveDirections;
            moveDirections = new Direction[4];
            moveDirections[0] = prevBestDirection;
            int directionIndex = 1;
            foreach (Direction direction in directions)
            {
                if (direction != moveDirections[0])
                {
                    moveDirections[directionIndex] = direction;
                }
                ++directionIndex;
            }
            return moveDirections;
        }

        public double AddSearch(int depth, double alpha, double beta)
        {
            int hashFlag = hashAlpha;
            double val;
            if ((val = ProbeHash(depth, alpha, beta, out Decision prevDecision)) != infinity)
            {
                ++curOff;
                return val;
            }
            if (depth == 0)
            {
                val = -Evaluator.EvalForMove(chessBoard);
                RecordHash(depth, val, hashExact, prevDecision);
                return val;
            }
            Position[] addPositions = SortedPositions(prevDecision);
            foreach (Position position in addPositions)
            {
                foreach (int level in addLevels)
                {
                    chessBoard.AddNew(position, level);
                    val = -MoveSearch(depth - 1, -beta, -alpha);
                    chessBoard.SetEmpty(position);
                    if (val >= beta)
                    {
                        RecordHash(depth, beta, hashBeta, prevDecision);
                        return beta;
                    }
                    if (val > alpha)
                    {
                        hashFlag = hashExact;
                        prevDecision.bestPosition = position;
                        alpha = val;
                    }
                }
            }
            RecordHash(depth, alpha, hashFlag, prevDecision);
            return alpha;
        }

        private Position[] SortedPositions(Decision prevDecision)
        {

            List<Position> emptyPositions = chessBoard.GetEmptyPositions();
            Position prevBestPosition = prevDecision.bestPosition;
            if (prevBestPosition is null)
            {
                return emptyPositions.ToArray();
            }
            Position[] addPositions;
            addPositions = new Position[emptyPositions.Count];
            addPositions[0] = prevBestPosition;
            int positionIndex = 1;
            foreach (Position position in emptyPositions)
            {
                if (position != addPositions[0])
                {
                    addPositions[positionIndex] = position;
                }
                ++positionIndex;
            }
            addPositions = emptyPositions.ToArray();
            return addPositions;
        }
    }
}
