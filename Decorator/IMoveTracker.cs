using Project2048.Core;

namespace Project2048.Decorator
{
    internal interface IMoveTracker
    {
        void OnEachMoveStart(ChessBoard chessBoard);
        void OnEachMoveEnd(ChessBoard chessBoard);
    }
}
