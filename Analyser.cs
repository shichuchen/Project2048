using System;
using System.Collections.Generic;
using System.Linq;

namespace Project2048
{
    class Analyser : IMoveTracker, IRoundTracker, ICompleteTracker
    {
        private static readonly int[] values = Chess.Values;
        private const int maxRound = Settings.MaxRound;
        private readonly Dictionary<int, int> valueCountMap = new Dictionary<int, int>();
        private TimeRecorder roundTimeRecorder;
        private TimeRecorder moveTimeRecorder;
        private int moveCount = 0;
        private double totalMoveMilliSeconds = 0;
        private static double roundTotalDepth = 0;
        private static double gameTotalDepth = 0;
        public static void StoreDepth(int depth)
        {
            roundTotalDepth += depth;
        }
        public void OnEachRoundStart(ChessBoard chessBoard)
        {

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
        public void PrintEndTime()
        {
            Console.WriteLine("\t移动次数为:\t{0}", moveCount);
            Console.WriteLine("\t每次移动平均用时:\t{0}ms", totalMoveMilliSeconds / moveCount);
            Console.WriteLine("\t每次移动平均搜索深度为:\t{0}", roundTotalDepth / moveCount);
            gameTotalDepth += roundTotalDepth / moveCount;
            Console.WriteLine("\t本局总共用时:\t{0}s", roundTimeRecorder.GetTotalSeconds());
        }
        public void OnEachRoundEnd(ChessBoard chessBoard)
        {
            RecordResult(chessBoard);
            PrintEndTime();
        }
        private void RecordResult(ChessBoard chessBoard)
        {
            chessBoard.CalculateValues();
            int maxValue = chessBoard.IncludedValues.Max();
            for (int level = 16; level > 7; --level)
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
                    valueCountMap[value] = Math.Min(valueCountMap[value], maxRound);
                }
            }
        }
        public void OnComplete()
        {
            Console.WriteLine("\t全局每次移动平均搜索深度为:\t{0}", gameTotalDepth / maxRound);
            List<int> reachedValues = valueCountMap.Keys.ToList();
            reachedValues.Sort((x, y) => y.CompareTo(x));
            for (int i = 0; i < reachedValues.Count; ++i)
            {
                int value = reachedValues[i];
                double possibility = (valueCountMap[reachedValues[i]] / (double)Settings.MaxRound) * 100;
                Console.WriteLine(
                    "\t{0}\t出现的概率为:\t{1:f}%\n", value, possibility
                    );
            }
        }
    }
}
