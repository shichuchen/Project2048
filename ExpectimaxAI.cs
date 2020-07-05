namespace Project2048
{
    using Direction = Settings.Direction;
    class ExpectimaxAI : IPlayer
    {
        private static readonly Direction[] directions = Settings.Directions;
        private static readonly int[] addLevels = Chess.AddLevels;
        private static readonly float levelTwoPossibility = Settings.LevelTwoPossiblity;
        public ExpectimaxAI(ChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }
        private readonly ChessBoard chessBoard;
        public Direction GetMoveDirection()
        {
            int depth = 2;
            double maxValue = -double.MaxValue;
            var BestDirection = Direction.None;
            foreach (Direction direction in directions)
            {
                var newBoard = chessBoard.Copy();
                if (newBoard.Move(direction))
                {
                    var eval = MoveStateEvaluation(newBoard, depth);
                    if (eval > maxValue)
                    {
                        maxValue = eval;
                        BestDirection = direction;
                    }
                }
            }
            return BestDirection;
        }
        public double MoveStateEvaluation(ChessBoard chessBoard, int depth)
        {
            if (depth == 0)
            {
                return Evaluator.EvalForMove(chessBoard);
            }
            double result = Evaluator.EvalForMove(chessBoard);
            var emptyPositions = chessBoard.CalculateAndGetEmptyPositions();
            foreach (int level in addLevels)
            {
                foreach (Position position in emptyPositions)
                {
                    chessBoard.AddNew(position, addLevels[0]);
                    result += AddStateEvaluation(chessBoard, depth - 1) * (1 - levelTwoPossibility);

                    chessBoard.AddNew(position, addLevels[1]);
                    result += AddStateEvaluation(chessBoard, depth - 1) * levelTwoPossibility;
                    chessBoard.SetEmpty(position);
                }
            }
            return result;
        }
        public double AddStateEvaluation(ChessBoard chessBoard, int depth)
        {
            if (depth == 0)
            {
                return Evaluator.EvalForMove(chessBoard);
            }
            double maxValue = -Evaluator.LostPenality;
            foreach (Direction direction in directions)
            {
                var newBoard = chessBoard.Copy();
                if (newBoard.Move(direction))
                {
                    var eval = MoveStateEvaluation(newBoard, depth);
                    if (eval > maxValue)
                    {
                        maxValue = eval;
                    }
                }
            }
            return maxValue;
        }
    }
}
