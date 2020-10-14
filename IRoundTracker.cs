namespace Project2048
{
    internal interface IRoundTracker
    {
        void OnEachRoundStart(ChessBoard chessBoard);
        void OnEachRoundEnd(ChessBoard chessBoard);
    }
}
