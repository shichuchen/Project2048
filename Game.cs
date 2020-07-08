namespace Project2048
{
    using Direction = Settings.Direction;
    class Game
    {
        private static readonly bool printProcess = Settings.PrintProcess;
        private const int maxRound = Settings.MaxRound;
        private static GameManager gameManager;
        private static ChessBoard chessBoard;
        static void Main()
        {
            gameManager = new GameManager();
            for (int round = 0; round < maxRound; ++round)
            {
                InitializeChessBoard();
                PlayGame();
            }
            gameManager.OnComplete();
        }

        private static void PlayGame()
        {
            while (true)
            {
                if (chessBoard.IsGameOver())
                {
                    chessBoard.Print();
                    gameManager.OnEachRoundEnd(chessBoard);
                    break;
                }
                var ai = new DecisionCacheAlphaBetaAI(chessBoard);
                gameManager.OnEachMoveStart(chessBoard);
                Direction direction = ai.GetMoveDirection();
                gameManager.OnEachMoveEnd(chessBoard);
                if (chessBoard.Move(direction))
                {
                    PrintProcess(chessBoard);
                    chessBoard.RandomAdd();
                    PrintProcess(chessBoard);

                }
            }
        }

        private static void InitializeChessBoard()
        {
            chessBoard = new ChessBoard();
            gameManager.OnEachRoundStart(chessBoard);
            chessBoard.RandomAdd();
            chessBoard.RandomAdd();
            PrintProcess(chessBoard);
        }

        private static void PrintProcess(ChessBoard chessBoard)
        {
            if (printProcess)
            {
                chessBoard.Print();
            }
        }
    }


}
