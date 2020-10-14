using System;

namespace Project2048
{
    using Line = UInt16;
    using Board = UInt64;
    /// <summary>
    /// 处理棋盘评估的分数
    /// </summary>
    public class Evaluator
    {
        static Evaluator()
        {
            CacheLineWeights();
        }
        public static readonly Weights Weights = new Weights();
        public const double infinity = double.MaxValue;
        private const int LineMaxValue = ChessBoard.lineMaxValue;
        private const Board RowMask = BitBoardHandler.rowMask;
        private static readonly double[] moveScores = new double[LineMaxValue];
        private static readonly double[] addScores = new double[LineMaxValue];
        /// <summary>
        /// 在每一轮开始前计算权重缓存结果
        /// </summary>
        public static void CacheLineWeights()
        {
            for (int line = 0; line < LineMaxValue; ++line)
            {
                GetCacheLineEmptySumMerges(line);
                GetCacheLineMono(line);
                CacheLineSmooth(line);
            }
        }
        private static void GetCacheLineEmptySumMerges(int line)
        {
            double sumPower = Weights.SumPower;
            var levels = BitBoardHandler.ToLevels((Line)line);
            double lineSum = 0;
            double lineEmpty = 0;
            double lineMerges = 0;

            int preLevel = 0;
            int counter = 0;
            for (int i = 0; i < 4; ++i)
            {
                int level = levels[i];
                lineSum += Math.Pow(level, sumPower);
                if (level == 0)
                {
                    ++lineEmpty;
                }
                else
                {
                    if (preLevel == level)
                    {
                        ++counter;
                    }
                    else if (counter > 0)
                    {
                        lineMerges += (1 + counter);
                        counter = 0;
                    }
                    preLevel = level;
                }
            }
            if (counter > 0)
            {
                lineMerges += (1 + counter);
            }
            moveScores[line] +=
                    lineEmpty * Weights.EmptyWeight
                    + lineMerges * Weights.MergeWeight
                    - lineSum * Weights.SumWeight;
        }
        private static void GetCacheLineMono(int line)
        {
            double monoPower = Weights.MoveMonoPower;
            var levels = BitBoardHandler.ToLevels((Line)line);
            double lineMonoLeft = 0;
            double lineMonoRight = 0;
            for (int i = 1; i < 4; ++i)
            {
                if (levels[i - 1] > levels[i])
                {
                    lineMonoLeft += Math.Pow(levels[i - 1], monoPower) - Math.Pow(levels[i], monoPower);
                }
                else
                {
                    lineMonoRight += Math.Pow(levels[i], monoPower) - Math.Pow(levels[i - 1], monoPower);
                }
            }
            moveScores[line] -= Math.Min(lineMonoLeft, lineMonoRight) * Weights.MonoWeight;
        }

        private static void CacheLineSmooth(int line)
        {
            double smoothPower = Weights.SmoothPower;
            var levels = BitBoardHandler.ToLevels((Line)line);
            double lineSmooth = 0;
            for (int i = 0; i < 3; ++i)
            {
                if (levels[i] > 0)
                {
                    for (int j = i + 1; j < 4; ++j)
                    {
                        if (levels[j] > 0)
                        {
                            lineSmooth -= Math.Pow(Math.Abs(levels[i] - levels[j]), smoothPower);
                            break;
                        }
                    }
                }

            }
            addScores[line] = lineSmooth * Weights.SmoothWeight;
        }

        public static void PrintWeights()
        {
            Console.WriteLine("\tEvaluator:");
            Console.WriteLine(Weights);
            Console.WriteLine("\n");
        }
        public static double EvalForMove(ChessBoard chessBoard)
        {
            var board = chessBoard.BitBoard;
            return GetScoresOfTables(board, moveScores) +
                GetScoresOfTables(board.ToTransposeLeft(), moveScores);
        }
        public static double EvalForAdd(ChessBoard chessBoard)
        {
            var board = chessBoard.BitBoard;
            return GetScoresOfTables(board, addScores) +
                GetScoresOfTables(board.ToTransposeLeft(), addScores);
        }

        private static double GetScoresOfTables(Board board, double[] scoreTables)
        {
            return scoreTables[board & RowMask] +
                               scoreTables[(board >> 16) & RowMask] +
                               scoreTables[(board >> 32) & RowMask] +
                               scoreTables[(board >> 48) & RowMask];
        }
    }
}
