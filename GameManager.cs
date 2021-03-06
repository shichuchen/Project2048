﻿using System;
using Project2048.Core;
using Project2048.Decorator;
using Project2048.Player;

namespace Project2048
{
    internal class GameManager : IMoveTracker, IRoundTracker, ICompleteTracker
    {
        private readonly bool onAnalyse = Settings.onAnalyse;
        private readonly Analyser analyser;
        private readonly bool onEvolve = Settings.onEvolve;
        private readonly AnnealingLearner learner;
        private int round;
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
            Console.WriteLine("第{0}次:", round);
            ++round;
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
