namespace Project2048
{
    using Direction = Settings.Direction;
    /// <summary>
    /// 不采用任何搜索算法, 只是简单的验证所有方向哪个方向对自己最优
    /// </summary>
    internal class ExpectimaxAi : IPlayer
    {
        private static readonly Direction[] directions = Settings.Directions;
        private static readonly int[] addLevels = ChessBoard.AddLevels;
        private static readonly double levelTwoPossibility = ChessBoard.levelTwoPossibility;
        private const double LostPenality = - Evaluator.infinity / 3;
        public ExpectimaxAi(ChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }
        private readonly ChessBoard chessBoard;
        public Direction GetMoveDirection()
        {
            int depth = 2;
            double maxValue = -double.MaxValue;
            var bestDirection = Direction.None;
            foreach (var direction in directions)
            {
                var newBoard = chessBoard.Copy();
                if (newBoard.Move(direction))
                {
                    var eval = MoveStateEvaluation(newBoard, depth);
                    if (eval > maxValue)
                    {
                        maxValue = eval;
                        bestDirection = direction;
                    }
                }
            }
            return bestDirection;
        }
        public double MoveStateEvaluation(ChessBoard newBoard, int depth)
        {
            if (depth == 0)
            {
                return Evaluator.EvalForMove(newBoard);
            }
            double result = Evaluator.EvalForMove(newBoard);
            var emptyPositions = newBoard.GetEmptyPositions();
            foreach (int level in addLevels)
            {
                foreach (var position in emptyPositions)
                {
                    newBoard.AddNew(position, addLevels[0]);
                    result += AddStateEvaluation(newBoard, depth - 1) * (1 - levelTwoPossibility);

                    newBoard.AddNew(position, addLevels[1]);
                    result += AddStateEvaluation(newBoard, depth - 1) * levelTwoPossibility;
                    newBoard.SetEmpty(position);
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
            double maxValue = LostPenality;
            foreach (var direction in directions)
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
