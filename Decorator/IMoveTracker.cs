namespace Project2048
{
    internal interface IMoveTracker
    {
        void OnEachMoveStart(ChessBoard chessBoard);
        void OnEachMoveEnd(ChessBoard chessBoard);
    }
}
