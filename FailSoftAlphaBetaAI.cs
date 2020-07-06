using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

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
        private Direction bestDirection = Direction.None;
        private int depth = 1;
        private static int cutOff = 0;
        public Direction GetMoveDirection()
        {
            cutOff = 0;
            hashTable = new Dictionary<Board, HashItem>();
            TimeRecorder timeRecorder = new TimeRecorder();
            while (timeRecorder.GetTotalMilliSeconds() <= maxMilliSecs)
            {
                MoveSearch(depth, -bound, bound);
                ++depth;
            }
            Analyser.StoreDepth(depth);
            Analyser.StoreCutOff(cutOff);
            if (printProcess)
            {
                Console.WriteLine("\tDepth:\t{0}", depth);
                Console.WriteLine("\tCutOff:\t{0}\n", cutOff);
            }
            return bestDirection;
        }
        private double ProbeHash(int depth, double alpha, double beta, out Decision prevDecision)
        {
            if (hashTable.TryGetValue(chessBoard.BitBoard, out var hashItem))
            {
                if (hashItem.depth >= depth)
                {
                    prevDecision = hashItem.decision;
                    if (hashItem.hashFlag == hashExact)
                    {
                        return hashItem.value;
                    }
                    if ((hashItem.hashFlag == hashAlpha) && (hashItem.value <= alpha))
                    {
                        return alpha;
                    }
                    if ((hashItem.hashFlag == hashBeta) && (hashItem.value >= beta))
                    {
                        return beta;
                    }
                }
            }
            prevDecision = new Decision();
            return infinity;
        }
        private void RecordHash(int depth, double value, int hashFlag, Decision decision)
        {
            if (hashTable.TryGetValue(chessBoard.BitBoard, out HashItem hashItem))
            {
                if (hashItem.depth > depth)
                {
                    return;
                }

            }
            hashTable[chessBoard.BitBoard] = new HashItem()
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
                ++cutOff;
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
            //Direction[] moveDirections;
            //if (prevDecision.bestDirection != Direction.None)
            //{
            //    moveDirections = new Direction[4];
            //    moveDirections[0] = prevDecision.bestDirection;
            //    int directionIndex = 1;
            //    foreach (Direction direction in directions)
            //    {   
            //        if(direction != moveDirections[0])
            //        {
            //            moveDirections[directionIndex] = direction;
            //        }
            //        ++directionIndex;
            //    }
            //}
            //else
            //{
            //    moveDirections = directions;
            //}
            foreach (Direction direction in directions)
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
        public double AddSearch(int depth, double alpha, double beta)
        {
            int hashFlag = hashAlpha;
            double val;
            if ((val = ProbeHash(depth, alpha, beta, out Decision prevDecision)) != infinity)
            {
                ++cutOff;
                return val;
            }
            if (depth == 0)
            {
                val = -Evaluator.EvalForMove(chessBoard);
                RecordHash(depth, val, hashExact, prevDecision);
                return val;
            }

            var emptyPositions = chessBoard.GetEmptyPositions();
            foreach (int level in addLevels)
            {
                foreach (Position position in emptyPositions)
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
                        alpha = val;
                    }
                }
            }
            RecordHash(depth, alpha, hashFlag, prevDecision);
            return alpha;
        }
    }
}
