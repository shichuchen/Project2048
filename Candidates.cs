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
        public Candidates(ChessBoard chessBoard)
        {
            this[addLevels[0]] = new List<Position>();
            this[addLevels[1]] = new List<Position>();
            //AllIn(chessBoard);
            ChooseAnnoyingChess(chessBoard);
        }
        private void AllIn(ChessBoard chessBoard)
        {
            var emptyPositions = chessBoard.GetEmptyPositions();
            this[addLevels[0]] = new List<Position>(emptyPositions);
            this[addLevels[1]] = new List<Position>(emptyPositions);
        }

        private double minEval = double.MaxValue;
        private static readonly int[] addLevels = ChessBoard.AddLevels;
        public int[] Levels { get { return addLevels; } }
        private void ChooseAnnoyingChess(ChessBoard chessBoard)
        {
            var emptyPositions = chessBoard.GetEmptyPositions();
            foreach (int level in addLevels)
            {
                foreach (Position position in emptyPositions)
                {
                    chessBoard.AddNew(position, level);
                    double eval = Evaluator.EvalForAdd(chessBoard);
                    //chessBoard.Print();
                    //Console.WriteLine("\t{0}", eval);
                    if (eval < minEval)
                    {
                        minEval = eval;
                        this[addLevels[0]].Clear();
                        this[addLevels[1]].Clear();

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
