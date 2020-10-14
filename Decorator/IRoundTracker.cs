using Project2048.Core;

namespace Project2048.Decorator
{
    internal interface IRoundTracker
    {
        void OnEachRoundStart(ChessBoard chessBoard);
        void OnEachRoundEnd(ChessBoard chessBoard);
    }
}
