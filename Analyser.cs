using System;
using System.Collections.Generic;
using System.Linq;

namespace Project2048
{   
    /// <summary>
    /// 分析棋盘数据
    /// </summary>
    class Analyser : IMoveTracker, IRoundTracker, ICompleteTracker
    {
        private const int maxRound = Settings.MaxRound;
        private readonly Dictionary<int, int> valueCountMap = new Dictionary<int, int>();
        private TimeRecorder roundTimeRecorder;
        private TimeRecorder moveTimeRecorder;
        private int moveCount = 0;
        private double totalMoveMilliSeconds = 0;
        private static double roundTotalDepth = 0;
        private static double gameTotalDepth = 0;
        private static int round = 0;

        private static double roundTotalCutOff = 0;
        private static double gameTotalCutOff = 0;
        public static void StoreDepth(int depth)
        {
            roundTotalDepth += depth;
        }
        public static void StoreCutOff(int cutOff)
        {
            roundTotalCutOff += cutOff;
        }
        public void OnEachRoundStart(ChessBoard chessBoard)
        {
            roundTotalCutOff = 0;
            roundTotalDepth = 0;
            moveCount = 0;
            totalMoveMilliSeconds = 0;
            roundTimeRecorder = new TimeRecorder();
        }
        public void OnEachMoveStart(ChessBoard chessBoard)
        {
            moveTimeRecorder = new TimeRecorder();
        }
        public void OnEachMoveEnd(ChessBoard chessBoard)
        {
            ++moveCount;
            totalMoveMilliSeconds += moveTimeRecorder.GetTotalMilliSeconds();
        }
        public void OnEachRoundEnd(ChessBoard chessBoard)
        {
            ++round;
            RecordResult(chessBoard);
            PrintSearcherDatas();
            PrintValuesPossibilities();
        }
        public void PrintSearcherDatas()
        {
            Console.WriteLine("\t移动次数为:\t{0}", moveCount);
            Console.WriteLine("\t平均用时:\t{0}ms", totalMoveMilliSeconds / moveCount);

            var roundAverageDepth = roundTotalDepth / moveCount;
            Console.WriteLine("\t平均搜索深度为:\t{0}", roundAverageDepth);
            gameTotalDepth += roundAverageDepth;

            var roundAverageCutOff = roundTotalCutOff / moveCount;
            Console.WriteLine("\t平均置换表裁剪为:\t{0}", roundAverageCutOff);
            gameTotalCutOff += roundAverageCutOff;

            Console.WriteLine("\t总共用时:\t{0}s", roundTimeRecorder.GetTotalSeconds());
        }
        private void PrintValuesPossibilities()
        {
            List<int> reachedValues = valueCountMap.Keys.ToList();
            reachedValues.Sort((x, y) => y.CompareTo(x));
            for (int i = 0; i < reachedValues.Count; ++i)
            {
                int value = reachedValues[i];
                double possibility = (valueCountMap[reachedValues[i]] / (double)round) * 100;
                Console.WriteLine(
                    "\t{0}\t出现的概率为:\t{1:f}%\n", value, possibility
                    );
            }
        }
        private void RecordResult(ChessBoard chessBoard)
        {
            int maxValue = chessBoard.MaxValue;
            for (int level = 16; level > 9; --level)
            {
                int value = 1 << level;
                if (value <= maxValue)
                {
                    if (valueCountMap.ContainsKey(value))
                    {
                        ++valueCountMap[value];
                    }
                    else
                    {
                        valueCountMap[value] = 1;
                    }
                    valueCountMap[value] = Math.Min(valueCountMap[value], round);
                }
            }
        }
        public void OnComplete()
        {
            Console.WriteLine("\t全局平均搜索深度为:\t{0}", gameTotalDepth / maxRound);
            Console.WriteLine("\t全局平均置换表裁剪为:\t{0}", gameTotalCutOff / maxRound);

        }


    }
}
