using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2048
{
    using Direction = Settings.Direction;
    public class FailSoftAlphaBetaAI : IPlayer
    {
        public FailSoftAlphaBetaAI(ChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }
        private ChessBoard chessBoard;
        private static readonly Direction[] directions = Settings.Directions;
        private static readonly int[] addLevels = ChessBoard.AddLevels;
        private const double infinity = Evaluator.Infinity;
        private const double lostPenality = Evaluator.LostPenality;
        private Direction bestDirection = Direction.None;
        private const int maxMilliSecs = Settings.MaxSearchMilliSecs;
        private int depth = 1;
        public Direction GetMoveDirection()
        {
            TimeRecorder timeRecorder = new TimeRecorder();
            while (timeRecorder.GetTotalMilliSeconds() <= maxMilliSecs)
            {
                MoveSearch(depth, -infinity, infinity);
                ++depth;
            }
            Analyser.StoreDepth(depth);
            return bestDirection;
        }
        public double MoveSearch(int depth, double alpha, double beta)
        {
            if (chessBoard.IsGameOver())
            {
                return lostPenality;
            }
            if (depth == 0)
            {
                return Evaluator.EvalForMove(chessBoard);
            }
            bool PVfind = false;
            foreach (Direction direction in directions)
            {
                var newBoard = chessBoard.Copy();
                if (newBoard.Move(direction))
                {
                    double val;
                    var ai = new FailSoftAlphaBetaAI(newBoard);
                    if (PVfind)
                    {
                        val = -ai.AddSearch(depth - 1, -alpha - 1, -alpha);
                        if (val > alpha && val < beta)
                        {
                            val = -ai.AddSearch(depth - 1, -beta, -alpha);
                        }
                    }
                    else
                    {
                        val = -ai.AddSearch(depth - 1, -beta, -alpha);
                    }
                    if (val >= alpha)
                    {
                        //PVfind = true;
                        alpha = val;
                        bestDirection = direction;
                    }
                    if (val >= beta)
                    {
                        break;
                    }
                }
            }
            return alpha;
        }
        public double AddSearch(int depth, double alpha, double beta)
        {
            bool PVfind = false;
            if (depth == 0)
            {
                return -Evaluator.EvalForMove(chessBoard);
            }
            var emptyPositions = chessBoard.CalculateAndGetEmptyPositions();
            foreach (int level in addLevels)
            {
                foreach (Position position in emptyPositions)
                {
                    chessBoard.AddNew(position, level);
                    double val = 0;
                    if (PVfind)
                    {
                        val = -MoveSearch(depth - 1, -alpha - 1, -alpha);
                        if (val > alpha && val < beta)
                        {
                            val = -MoveSearch(depth - 1, -beta, -alpha);
                        }
                    }
                    else
                    {
                        val = -MoveSearch(depth - 1, -beta, -alpha);
                    }
                    chessBoard.SetEmpty(position);
                    if (val >= alpha)
                    {
                        //PVfind = true;
                        alpha = val;
                    }
                    if (val >= beta)
                    {
                        break;
                    }
                }
            }
            return alpha;
        }
    }
}
