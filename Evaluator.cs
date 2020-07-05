using System;

namespace Project2048
{
    using Line = UInt16;
    /// <summary>
    /// 处理棋盘评估的分数
    /// </summary>
    public class Evaluator
    {   
        static Evaluator()
        {
            CacheLineWeights();
        }
        public static readonly Weights weights = new Weights();
        public const double Infinity = double.MaxValue;
        public const double LostPenality = -Infinity/ 3;
        private const int LineMaxValue = 65536;
        private static readonly double[] moveScores = new double[LineMaxValue];
        private static readonly double[] addScores = new double[LineMaxValue];
        /// <summary>
        /// 在每一轮开始前计算权重缓存结果
        /// </summary>
        public static void CacheLineWeights()
        {
            double sumPower = weights.SumPower;
            for (int line = 0; line < LineMaxValue; ++line)
            {
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

                GetCacheLineMono(line);
                moveScores[line] =
                    lineEmpty * weights.EmptyWeight
                    + lineMerges * weights.MergeWeight
                    - GetCacheLineMono(line)
                    - lineSum * weights.SumWeight;
                CacheLineSmooth(line);
            }
        }

        private static double GetCacheLineMono(int line)
        {
            double monoPower = weights.MoveMonoPower;
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
            return Math.Min(lineMonoLeft, lineMonoRight) * weights.MonoWeight;
        }

        private static void CacheLineSmooth(int line)
        {
            double smoothPower = weights.SmoothPower;
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
            addScores[line] = lineSmooth * weights.SmoothWeight;
        }

        public static void PrintWeights()
        {
            Console.WriteLine("\tEvaluator:");
            Console.WriteLine(weights);
            Console.WriteLine("\n");
        }
        public static double EvalForMove(ChessBoard chessBoard)
        {
            return chessBoard.GetScoresOfTables(moveScores);
        }
        public static double EvalForAdd(ChessBoard chessBoard)
        {
            return chessBoard.GetScoresOfTables(addScores);
        }

    }
}
