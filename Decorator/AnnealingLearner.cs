﻿using System;
using Project2048.Core;
using Project2048.Player;

namespace Project2048.Decorator
{   
    /// <summary>
    /// 利用退火算法改变棋盘参数, 机器学习
    /// </summary>
    internal class AnnealingLearner : ILearner
    {
        public AnnealingLearner()
        {
            weightChangeCnt = Math.Min(weightChangeCnt, totalChangeCnt);
        }
        private static readonly int[] weightsChangeIndices = { 1, 2, 3, };
        private static readonly int totalChangeCnt = weightsChangeIndices.Length;
        private const float StartTemp = 100;
        private const float EndTemp = 0.1f;
        private const float DecreaseRate = 0.9f;
        /// <summary>
        /// 一次接受的扰动所改变的权重数量
        /// </summary>
        private static int weightChangeCnt = 2;
        /// <summary>
        /// 单一温度迭代次数
        /// </summary>
        private const int TempIterCount = 10;
        /// <summary>
        /// 对一定数量进行评估, 消除由随机性产生的一定误差
        /// </summary>
        private const int BoardIterCnt = 2;
        /// <summary>
        /// 2048 + 1024
        /// </summary>
        private const int OneRoundStartScore = 35000;

        private float temperature = StartTemp;
        private float prevScore = OneRoundStartScore * BoardIterCnt;
        private int currTempIterCount;
        private int currBoardIterCount;
        private int currentScore;
        private Weights weights = new Weights();

        public bool IsEnd => temperature <= EndTemp;
        public void OnEachRoundStart(ChessBoard chessBoard) { }
        public void OnEachRoundEnd(ChessBoard chessBoard)
        {
            UpdateIter(chessBoard);
            if (currBoardIterCount >= BoardIterCnt)
            {
                UpdateBestWeights();
                ChangeWeights();
                if (currTempIterCount >= TempIterCount)
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
                weights = Evaluator.Weights;
                PrintWeights();
            }
        }

        private void DecreaseTemperature()
        {
            temperature *= DecreaseRate;
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
            var indices = RandomGenerator.GetDistinctInts(weightChangeCnt, totalChangeCnt);
            var rands = RandomGenerator.GetDoubles(weightChangeCnt);
            for (int i = 0; i < weightChangeCnt; ++i)
            {
                double interp = GetDisturbanceFactor(rands[i]);
                int weightsChangeIndex = weightsChangeIndices[indices[i]];
                SetEvaluatorWeights(weightsChangeIndex, interp);
            }
        }
        private void SetEvaluatorWeights(int heurIndex, double interp)
        {
            double minWeight = Evaluator.Weights[heurIndex].Min;
            double maxWeight = Evaluator.Weights[heurIndex].Max;
            Evaluator.Weights.SetDeltaChangeToWeight(heurIndex, interp * (maxWeight - minWeight));
        }


    }
}
