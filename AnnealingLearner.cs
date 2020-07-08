using System;

namespace Project2048
{   
    /// <summary>
    /// 利用退火算法改变棋盘参数, 机器学习
    /// </summary>
    class AnnealingLearner : ILearner
    {
        public AnnealingLearner()
        {
            weightChangeCnt = Math.Min(weightChangeCnt, totalChangeCnt);
        }
        private static readonly int[] weightsChangeIndices = { 1, 2, 3, };
        private static readonly int totalChangeCnt = weightsChangeIndices.Length;
        private const float startTemp = 100;
        private const float endTemp = 0.1f;
        private const float decreaseRate = 0.9f;
        /// <summary>
        /// 一次接受的扰动所改变的权重数量
        /// </summary>
        private static int weightChangeCnt = 2;
        /// <summary>
        /// 单一温度迭代次数
        /// </summary>
        private const int tempIterCount = 10;
        /// <summary>
        /// 对一定数量进行评估, 消除由随机性产生的一定误差
        /// </summary>
        private const int boardIterCnt = 2;
        /// <summary>
        /// 2048 + 1024
        /// </summary>
        private const int oneRoundStartScore = 35000;

        private float temperature = startTemp;
        private float prevScore = oneRoundStartScore * boardIterCnt;
        private int currTempIterCount = 0;
        private int currBoardIterCount = 0;
        private int currentScore = 0;
        private Weights weights = new Weights();

        public bool IsEnd { get { return temperature <= endTemp; } }
        public void OnEachRoundStart(ChessBoard chessBoard) { }
        public void OnEachRoundEnd(ChessBoard chessBoard)
        {
            UpdateIter(chessBoard);
            if (currBoardIterCount >= boardIterCnt)
            {
                UpdateBestWeights();
                ChangeWeights();
                if (currTempIterCount >= tempIterCount)
                {
                    DecreaseTemperature();
                }
            }
        }

        private void UpdateIter(ChessBoard chessBoard)
        {
            ++currTempIterCount;
            ++currBoardIterCount;
            currentScore += GetScore(chessBoard);
        }
        public int GetScore(ChessBoard chessBoard)
        {
            return chessBoard.Score;
        }
        private void UpdateBestWeights()
        {
            int score = currentScore;
            currentScore = 0;
            currBoardIterCount = 0;
            float deltaE = (score - prevScore) / 1000f;
            if (AcceptNewChange(deltaE))
            {
                prevScore = score;
                weights = Evaluator.weights;
                PrintWeights();
            }
        }

        private void DecreaseTemperature()
        {
            temperature *= decreaseRate;
            currTempIterCount = 0;
            Console.WriteLine("当前温度为:\t{0}", temperature);
        }

        public bool AcceptNewChange(double deltaE)
        {
            if (deltaE > 0)
            {
                return true;
            }
            else
            {
                double p = Math.Exp(deltaE / temperature);
                if (p > RandomGenerator.NextDouble())
                {
                    return true;
                }
                return false;
            }
        }
        public void PrintWeights()
        {
            Console.WriteLine("\tAnnealingLearner:");
            Console.WriteLine(weights);
            Console.WriteLine("\n");
        }
        /// <summary>
        /// 计算快速退火的扰动因子
        /// </summary>
        /// <param name="random">介于0到1之间的随机数</param>
        /// <returns></returns>
        private double GetDisturbanceFactor(double random)
        {
            return temperature * Math.Sign(random - 0.5) * (Math.Pow(1 + 1 / temperature, Math.Abs(2 * random - 1)) - 1);
        }
        public void ChangeWeights()
        {
            int[] indices = RandomGenerator.GetDistinctInts(weightChangeCnt, totalChangeCnt);
            double[] rands = RandomGenerator.GetDoubles(weightChangeCnt);
            for (int i = 0; i < weightChangeCnt; ++i)
            {
                double interp = GetDisturbanceFactor(rands[i]);
                int weightsChangeIndex = weightsChangeIndices[indices[i]];
                SetEvaluatorWeights(weightsChangeIndex, interp);
            }
        }
        private void SetEvaluatorWeights(int heurIndex, double interp)
        {
            double minWeight = Evaluator.weights[heurIndex].Min;
            double maxWeight = Evaluator.weights[heurIndex].Max;
            Evaluator.weights.SetDeltaChangeToWeight(heurIndex, interp * (maxWeight - minWeight));
        }


    }
}
