namespace Project2048
{
    interface IRoundTracker
    {
        void OnEachRoundStart(ChessBoard chessBoard);
        void OnEachRoundEnd(ChessBoard chessBoard);
    }
}
