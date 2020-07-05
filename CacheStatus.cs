using System.Collections.Generic;
using System.Linq;

namespace Project2048
{
    class CacheStatus<TDecision, TStatus>
        where TStatus : class, new()
    {
        public CacheStatus(TDecision initBestDecision)
        {
            BestDecision = initBestDecision;
            this.initBestDecision = initBestDecision;
        }
        private Dictionary<ChessBoard, TDecision> boardDecisionMap;
        private ChessBoard[] sortedBoards;
        private Dictionary<ChessBoard, TStatus> boardStatusMap;
        private readonly TDecision initBestDecision;
        private TDecision bestDecision;
        public TDecision BestDecision
        {
            get
            {
                return bestDecision;
            }
            set
            {
                //{   if(Equals(bestDecision,value))
                //    {
                //        return;
                //    }
                bestDecision = value;
                //if (!(boardDecisionMap is null))
                //{
                //    SortBoards();
                //}                              
            }
        }
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
            if (boardStatusMap is null)
            {
                return new ChessBoard[] { };
            }
            if (Equals(initBestDecision, BestDecision))
            {
                return boardStatusMap.Keys.ToArray();
            }
            else
            {
                SortBoards();
                return sortedBoards;
            }
        }
        public void SortBoards()
        {
            var boardKeys = boardDecisionMap.Keys;
            var boards = boardKeys.ToArray();
            var tempSortedBoards = new ChessBoard[boards.Length];
            int sortedIndex = 1;
            foreach (var chessBoard in boardKeys)
            {
                if (tempSortedBoards[0] is null &&
                    Equals(boardDecisionMap[chessBoard], BestDecision))
                {
                    tempSortedBoards[0] = chessBoard;
                }
                else
                {
                    tempSortedBoards[sortedIndex] = chessBoard;
                    ++sortedIndex;
                }
            }
            if (sortedBoards is null)
            {
                sortedBoards = new ChessBoard[boards.Length];
            }
            sortedBoards = tempSortedBoards;
        }
        public TDecision GetDecision(ChessBoard chessBoard)
        {
            return boardDecisionMap[chessBoard];
        }
    }
}
