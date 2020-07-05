﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2048
{
    using Direction = Settings.Direction;
    public class FailSoftAlphaBetaAI : IPlayer
    {   
        public FailSoftAlphaBetaAI(ChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }
        private ChessBoard chessBoard;
        private static readonly Direction[] directions = Settings.Directions;
        private static readonly int[] addLevels = Chess.AddLevels;
        private const double infinity = Evaluator.Infinity;
        private const double lostPenality = Evaluator.LostPenality;
        private Direction bestDirection = Direction.None;
        public Direction GetMoveDirection()
        {
            throw new NotImplementedException();
        }
        public double MoveStateEvaluation(int depth, double alpha, double beta)
        {
            throw new NotImplementedException();
        }
        public double AddStateEvaluation(int depth, double alpha, double beta)
        {
            throw new NotImplementedException();
        }
    }
}
