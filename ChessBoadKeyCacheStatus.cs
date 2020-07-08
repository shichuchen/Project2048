using System.Collections.Generic;
using System.Linq;

namespace Project2048
{
    class ChessBoadKeyCacheStatus<TDecision, TStatus>
        where TStatus : class, new()
    {
        public ChessBoadKeyCacheStatus(TDecision initBestDecision)
        {
            BestDecision = initBestDecision;
            this.initBestDecision = initBestDecision;
        }
        private Dictionary<ChessBoard, TDecision> boardDecisionMap;
        private Dictionary<ChessBoard, TStatus> boardStatusMap;
        private readonly TDecision initBestDecision;
        private ChessBoard[] sortedBoards;

        public TDecision BestDecision { get; set; }
        public void AddBoard(TDecision decision, ChessBoard chessBoard)
        {
            if (boardStatusMap is null)
            {
                boardStatusMap = new Dictionary<ChessBoard, TStatus>();
            }
            boardStatusMap[chessBoard] = new TStatus();
            if (boardDecisionMap is null)
            {
                boardDecisionMap = new Dictionary<ChessBoard, TDecision>();
            }
            boardDecisionMap[chessBoard] = decision;
        }
        public TStatus GetStatus(ChessBoard chessBoard)
        {
            return boardStatusMap[chessBoard];
        }
        public ChessBoard[] GetBoards()
        {
            if (Equals(initBestDecision, BestDecision))
            {   
                if(sortedBoards is null)
                {
                    sortedBoards = boardDecisionMap.Keys.ToArray();
                }
            }
            else
            {
                SortBoards();
            }
            return sortedBoards;
        }
        public void  SortBoards()
        {   
            if(Equals(sortedBoards[0], BestDecision))
            {
                return;
            }
            Dictionary<ChessBoard, TDecision>.KeyCollection boardKeys = boardDecisionMap.Keys;
            ChessBoard[] boards = boardKeys.ToArray();
            sortedBoards = new ChessBoard[boards.Length];
            int sortedIndex = 1;
            foreach (ChessBoard chessBoard in boardKeys)
            {
                if (sortedBoards[0] is null &&
                    Equals(boardDecisionMap[chessBoard], BestDecision))
                {
                    sortedBoards[0] = chessBoard;
                }
                else
                {
                    if (Equals(boardDecisionMap[chessBoard], BestDecision) && sortedIndex > 1)
                    {
                        var tempBoard = sortedBoards[1];
                        sortedBoards[1] = chessBoard;
                        sortedBoards[sortedIndex] = tempBoard;
                    }
                    else
                    {
                        sortedBoards[sortedIndex] = chessBoard;
                    }
                    ++sortedIndex;
                }
            }
        }
        public TDecision GetDecision(ChessBoard chessBoard)
        {
            return boardDecisionMap[chessBoard];
        }
    }
}
