using Project2048.Core;

namespace Project2048.Decorator
{
    internal interface ILearner : IRoundTracker
    {
        void ChangeWeights();
        int GetScore(ChessBoard chessBoard);
    }
}
