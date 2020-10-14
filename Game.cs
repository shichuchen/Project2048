using Project2048.Core;
using Project2048.Player;

namespace Project2048
{
    internal class Game
    {
        private static readonly bool printProcess = Settings.printProcess;
        private const int MaxRound = Settings.maxRound;
        private static GameManager gameManager;
        private static ChessBoard chessBoard;

        private static void Main()
        {
            gameManager = new GameManager();
            for (int round = 0; round < MaxRound; ++round)
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
                var ai = new DecisionCacheAlphaBetaAi(chessBoard);
                gameManager.OnEachMoveStart(chessBoard);
                var direction = ai.GetMoveDirection();
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
