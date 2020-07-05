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
                if (gameManager.IsEnd())
                {
                    break;
                }
                Console.WriteLine("第{0}次:", round);
                ChessBoard chessBoard = new ChessBoard();
                gameManager.OnEachRoundStart(chessBoard);
                chessBoard.RandomAdd();
                chessBoard.RandomAdd();
                if (printProcess)
                {
                    chessBoard.Print();
                }
                while (true)
                {
                    if (chessBoard.IsGameOver())
                    {
                        chessBoard.Print();
                        gameManager.OnEachRoundEnd(chessBoard);
                        break;
                    }
                    var ai = new AlphaBetaAI(chessBoard);
                    gameManager.OnEachMoveStart(chessBoard);
                    Direction direction = ai.GetMoveDirection();
                    gameManager.OnEachMoveEnd(chessBoard);
                    if (chessBoard.Move(direction))
                    {
                        if (printProcess)
                        {
                            chessBoard.Print();
                        }

                        chessBoard.RandomAdd();
                        if (printProcess)
                        {
                            chessBoard.Print();
                        }
                    }
                }
            }
            gameManager.OnComplete();
        }
    }


}
