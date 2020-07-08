using System;

namespace Project2048
{
    using Direction = Settings.Direction;
    class Game
    {
        private static readonly bool printProcess = Settings.PrintProcess;
        private const int maxRound = Settings.MaxRound;
        static void Main()
        {
            GameManager gameManager = new GameManager();
            for (int round = 0; round < maxRound; ++round)
            {
                Console.WriteLine("第{0}次:", round);
                ChessBoard chessBoard = new ChessBoard();
                gameManager.OnEachRoundStart(chessBoard);
                chessBoard.RandomAdd();
                chessBoard.RandomAdd();
                PrintProcess(chessBoard);
                while (true)
                {
                    if (chessBoard.IsGameOver())
                    {
                        chessBoard.Print();
                        gameManager.OnEachRoundEnd(chessBoard);
                        break;
                    }
                    var ai = new HashCacheAlphaBetaAI(chessBoard);
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
            gameManager.OnComplete();
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
