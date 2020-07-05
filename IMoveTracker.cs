namespace Project2048
{
    interface IMoveTracker
    {
        void OnEachMoveStart(ChessBoard chessBoard);
        void OnEachMoveEnd(ChessBoard chessBoard);
    }
}
