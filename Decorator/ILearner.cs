namespace Project2048
{
    internal interface ILearner : IRoundTracker
    {
        void ChangeWeights();
        int GetScore(ChessBoard chessBoard);
    }
}
