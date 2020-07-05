namespace Project2048
{
    interface ILearner : IRoundTracker
    {
        void ChangeWeights();
        int GetScore(ChessBoard chessBoard);
    }
}
