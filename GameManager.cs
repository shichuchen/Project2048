using System;

namespace Project2048
{
    class GameManager : IMoveTracker, IRoundTracker, ICompleteTracker
    {
        private readonly bool onAnalyse = Settings.OnAnalyse;
        private readonly Analyser analyser;
        private readonly bool onEvolve = Settings.OnEvolve;
        private readonly AnnealingLearner learner;
        public GameManager()
        {
            if (onAnalyse)
            {
                analyser = new Analyser();
            }
            if (onEvolve)
            {
                learner = new AnnealingLearner();
            }
        }
        public bool IsEnd()
        {
            if (learner is null)
            {
                return false;
            }
            return learner.IsEnd;
        }

        public void OnComplete()
        {
            if (onAnalyse)
            {
                analyser.OnComplete();

            }
            if (onEvolve)
            {
                learner.PrintWeights();
            }
            Evaluator.PrintWeights();

        }

        public void OnEachMoveEnd(ChessBoard chessBoard)
        {
            if (onAnalyse)
            {
                analyser.OnEachMoveEnd(chessBoard);
            }
        }

        public void OnEachMoveStart(ChessBoard chessBoard)
        {
            if (onAnalyse)
            {
                analyser.OnEachMoveStart(chessBoard);
            }
        }

        public void OnEachRoundEnd(ChessBoard chessBoard)
        {
            if (onAnalyse)
            {
                analyser.OnEachRoundEnd(chessBoard);
            }
            if (onEvolve)
            {
                learner.OnEachRoundEnd(chessBoard);
            }
            Console.WriteLine("\n\n");
        }

        public void OnEachRoundStart(ChessBoard chessBoard)
        {
            if (onAnalyse)
            {
                analyser.OnEachRoundStart(chessBoard);
            }
            if (onEvolve)
            {
                learner.OnEachRoundStart(chessBoard);
            }
            Evaluator.CacheLineWeights();
        }
    }
}
