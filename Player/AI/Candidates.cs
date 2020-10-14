using System;
using System.Collections.Generic;

namespace Project2048
{
    /// <summary>
    /// 在搜索环节需要预测对方落子的位置
    /// 该类负责处理并返回对方可能的落子区域
    /// </summary>
    public class Candidates : Dictionary<int, List<Position>>
    {
        public static Candidates AllIn(ChessBoard chessBoard)
        {
            var emptyPositions = chessBoard.GetEmptyPositions();
            var candidates = new Candidates
            {
                [AddLevels[0]] = new List<Position>(emptyPositions),
                [AddLevels[1]] = new List<Position>(emptyPositions)
            };
            return candidates;
        }
        public static Candidates ChooseAnnoying(ChessBoard chessBoard)
        {
            var candidates = new Candidates();
            candidates.ChooseAnnoyingChess(chessBoard);
            return candidates;
        }
        private double minEval = double.MaxValue;
        protected static readonly int[] AddLevels = ChessBoard.AddLevels;
        public int[] Levels => AddLevels;

        private void ChooseAnnoyingChess(ChessBoard chessBoard)
        {
            this[AddLevels[0]] = new List<Position>();
            this[AddLevels[1]] = new List<Position>();
            var emptyPositions = chessBoard.GetEmptyPositions();
            foreach (int level in AddLevels)
            {
                foreach (var position in emptyPositions)
                {
                    chessBoard.AddNew(position, level);
                    double eval = Evaluator.EvalForAdd(chessBoard);
                    if (eval < minEval)
                    {
                        minEval = eval;
                        this[AddLevels[0]].Clear();
                        this[AddLevels[1]].Clear();

                    }

                    if (eval == minEval)
                    {
                        this[level].Add(position);
                    }

                    chessBoard.SetEmpty(position);
                }
            }
        }

        public void Print()
        {
            foreach (var level in Levels)
            {
                Console.WriteLine("\tlevel:\t{0}\n", level);
                foreach (var position in this[level])
                {
                    Console.WriteLine("\t({0}, {1})\n", position.Row, position.Col);
                }
            }
        }
    }
}
