using System;
using System.Collections.Generic;

namespace Project2048
{
    using Direction = Settings.Direction;
    using Board = UInt64;
    public class FailSoftAlphaBetaAi : IPlayer
    {
        public class Decision
        {
            public Direction bestDirection = Direction.None;
            public Position bestPosition;
        }
        public class HashItem
        {
            public int depth;
            public int hashFlag = -1;
            public double value;
            public Decision decision = new Decision();

        }
        public FailSoftAlphaBetaAi(ChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }

        private readonly ChessBoard chessBoard;
        private static readonly Direction[] directions = Settings.Directions;
        private static readonly bool printProcess = Settings.printProcess;
        private static readonly int[] addLevels = ChessBoard.AddLevels;
        private const double Infinity = Evaluator.infinity;
        private const double Bound = Infinity / 2;
        private const double LostPenality = -Infinity / 3;
        private const int MaxMilliSecs = Settings.maxSearchMilliSecs;
        private const int HashExact = 0;
        private const int HashAlpha = 1;
        private const int HashBeta = 2;
        private static Dictionary<Board, HashItem> hashTable;
        private static int targetDepth;
        private static int depth = 1;
        private static int curOff;
        private Direction bestDirection = Direction.None;
        private static TimeRecorder timeRecorder;


        public Direction GetMoveDirection()
        {
            Initialize();
            targetDepth = Math.Max(6, (chessBoard.DistinctCount - 2) * 2);
            while (NotComplete())
            {
                MoveSearch(depth, -Bound, Bound);
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
            return timeRecorder.GetTotalMilliSeconds() <= MaxMilliSecs
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
                        case HashExact:
                            return hashItem.value;
                        case HashAlpha:
                            if (hashItem.value <= alpha)
                            {
                                return alpha;
                            }
                            break;
                        case HashBeta:
                            if (hashItem.value >= beta)
                            {
                                return beta;
                            }
                            break;
                    }
                }
            }
            prevDecision = new Decision();
            return Infinity;
        }
        private void RecordHash(int depth, double value, int hashFlag, Decision decision)
        {
            Board board = chessBoard.BitBoard;
            if (hashTable.TryGetValue(board, out var hashItem))
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
            int hashFlag = HashAlpha;
            double val;
            if ((val = ProbeHash(depth, alpha, beta, out var prevDecision)) != Infinity)
            {
                ++curOff;
                return val;
            }
            if (chessBoard.IsGameOver())
            {
                return LostPenality + depth;
            }
            if (depth == 0)
            {
                val = Evaluator.EvalForMove(chessBoard);
                RecordHash(depth, val, HashExact, prevDecision);
                return val;
            }
            var moveDirections = SortedDirections(prevDecision);
            foreach (var direction in moveDirections)
            {
                var newBoard = chessBoard.Copy();
                if (newBoard.Move(direction))
                {
                    var ai = new FailSoftAlphaBetaAi(newBoard);
                    val = -ai.AddSearch(depth - 1, -beta, -alpha);
                    if (val >= beta)
                    {
                        RecordHash(depth, beta, HashBeta, prevDecision);
                        return beta;
                    }
                    if (val > alpha)
                    {
                        hashFlag = HashExact;
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

            var moveDirections = new Direction[4];
            moveDirections[0] = prevBestDirection;
            int directionIndex = 1;
            foreach (var direction in directions)
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
            int hashFlag = HashAlpha;
            double val;
            if ((val = ProbeHash(depth, alpha, beta, out var prevDecision)) != Infinity)
            {
                ++curOff;
                return val;
            }
            if (depth == 0)
            {
                val = -Evaluator.EvalForMove(chessBoard);
                RecordHash(depth, val, HashExact, prevDecision);
                return val;
            }
            var addPositions = SortedPositions(prevDecision);
            foreach (var position in addPositions)
            {
                foreach (int level in addLevels)
                {
                    chessBoard.AddNew(position, level);
                    val = -MoveSearch(depth - 1, -beta, -alpha);
                    chessBoard.SetEmpty(position);
                    if (val >= beta)
                    {
                        RecordHash(depth, beta, HashBeta, prevDecision);
                        return beta;
                    }
                    if (val > alpha)
                    {
                        hashFlag = HashExact;
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

            var emptyPositions = chessBoard.GetEmptyPositions();
            var prevBestPosition = prevDecision.bestPosition;
            if (prevBestPosition is null)
            {
                return emptyPositions.ToArray();
            }

            var addPositions = new Position[emptyPositions.Count];
            addPositions[0] = prevBestPosition;
            int positionIndex = 1;
            foreach (var position in emptyPositions)
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
