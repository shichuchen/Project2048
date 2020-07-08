using System;
using System.Collections.Generic;
using System.Linq;

namespace Project2048
{
    using Direction = Settings.Direction;
    using Board = UInt64;
    public class BoardsCacheAlphaBetaAI : ISearcher
    {
        public BoardsCacheAlphaBetaAI(ChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }
        private readonly ChessBoard chessBoard;
        private static readonly bool printProcess = Settings.PrintProcess;
        private static readonly Direction[] directions = Settings.Directions;
        private const double infinity = Evaluator.Infinity;
        private const double lostPenality = -infinity / 3;
        private const double bound = infinity / 2;
        private static int CutOff = 0;
        public int Depth { get; set; }

        public class SearchState
        {
            public SearchState() { }
            private static readonly int searchMilliSecs = Settings.MaxSearchMilliSecs;

            private const int minDepth = 6;
            private const Direction initBestDirection = Direction.None;
            private const Position initBestPosition = null;
            private static int TargetDepth = minDepth;

            private ChessBoadKeyCacheStatus<Direction, SearchState> directionStatus;
            private ChessBoadKeyCacheStatus<Position, SearchState> positionStatus;
            private TimeRecorder timeRecorder;
            private bool VisitedThisState = false;

            public int Depth { get; set; } = 1;
            public int Turn { get; set; } = 0;
            public double Alpha { get; set; } = -bound;
            public double Beta { get; set; } = bound;
            public bool OnMove { get { return (Turn & 1) == 0; } }
            public Direction BestDirection
            {
                get { return directionStatus.BestDecision; }
                set { directionStatus.BestDecision = value; }
            }
            public Position BestPosition
            {
                get { return positionStatus.BestDecision; }
                set { positionStatus.BestDecision = value; }
            }
            public void Initialize(ChessBoard chessBoard)
            {
                StartTimeRecord();
                //InitDepth(chessBoard);
            }
            private void InitDepth(ChessBoard chessBoard)
            {
                TargetDepth = Math.Max(minDepth, (chessBoard.DistinctCount - 2) * 2);
            }
            public void StartTimeRecord()
            {
                timeRecorder = new TimeRecorder();
            }
            public bool TimeOut()
            {
                return 
                    timeRecorder.GetTotalMilliSeconds() >= searchMilliSecs
                    //&& Depth >= TargetDepth
                    ;
            }
            public double GetSearchTime()
            {
                if (timeRecorder is null)
                {
                    throw new ArgumentNullException("Please initialize SearchState Before Searching");
                }
                return timeRecorder.GetTotalMilliSeconds();
            }
            public void ToNextRound()
            {
                ++Depth;
                Alpha = -bound;
                Beta = bound;
            }
            public void TryBuildMoveStatus(ChessBoard chessBoard)
            {
                if (!VisitedThisState)
                {
                    VisitedThisState = true;
                    directionStatus = new ChessBoadKeyCacheStatus<Direction, SearchState>(initBestDirection);
                    foreach (Direction direction in directions)
                    {
                        ChessBoard newBoard = chessBoard.Copy();
                        TryAddDirectionStatus(direction, newBoard);
                    }
                }
            }

            private void TryAddDirectionStatus(Direction direction, ChessBoard newBoard)
            {
                if (newBoard.Move(direction))
                {
                    directionStatus.AddBoard(direction, newBoard);
                }
            }

            public void TryBuildAddStatus(ChessBoard chessBoard)
            {
                if (!VisitedThisState)
                {
                    VisitedThisState = true;
                    Candidates candidates = Candidates.AllIn(chessBoard);
                    positionStatus = new ChessBoadKeyCacheStatus<Position, SearchState>(initBestPosition);
                    foreach (int level in candidates.Levels)
                    {
                        foreach (Position position in candidates[level])
                        {
                            ChessBoard newBoard = chessBoard.Copy();
                            newBoard.AddNew(position, level);
                            positionStatus.AddBoard(position, newBoard);
                        }
                    }
                }
            }
            public ChessBoard[] GetBoards()
            {
                if (directionStatus is null)
                {
                    return positionStatus.GetBoards();
                }
                else
                {
                    return directionStatus.GetBoards();
                }
            }
            public Direction GetDirection(ChessBoard chessBoard)
            {
                return directionStatus.GetDecision(chessBoard);
            }
            public Position GetPosition(ChessBoard chessBoard)
            {
                return positionStatus.GetDecision(chessBoard);
            }
            public SearchState GetState(ChessBoard chessBoard)
            {
                if (directionStatus is null)
                {
                    SearchState newState = positionStatus.GetStatus(chessBoard);
                    SetNextState(newState);
                    return newState;
                }
                else
                {
                    SearchState newState = directionStatus.GetStatus(chessBoard);
                    SetNextState(newState);
                    return newState;
                }
            }
            public void SetNextState(SearchState newState)
            {
                newState.Alpha = -Beta;
                newState.Beta = -Alpha;
                newState.Turn = Turn + 1;
                newState.Depth = Depth - 1;
            }          
        }        
        public Direction GetMoveDirection()
        {
            CutOff = 0;
            SearchState searchState = new SearchState();
            searchState.Initialize(chessBoard);
            while (!searchState.TimeOut())
            {
                Depth = searchState.Depth;
                AlphaBeta(searchState);
                searchState.ToNextRound();
            }
            AnalyserRecordResult(searchState);
            PrintProcess(searchState);
            return searchState.BestDirection;
        }

        private static void AnalyserRecordResult(SearchState searchState)
        {
            Analyser.StoreDepth(searchState.Depth);
            Analyser.StoreCutOff(CutOff);
        }

        private void PrintProcess(SearchState searchState)
        {
            if (printProcess)
            {
                Console.WriteLine("\tDirection:\t{0}", searchState.BestDirection);
                Console.WriteLine("\tDepth:\t{0}", Depth);
                Console.WriteLine("\tCutOff:\t{0}", CutOff);
                Console.WriteLine("\tSearchTime:\t{0}\n", searchState.GetSearchTime());
            }
        }
        public double AlphaBeta(SearchState searchState)
        {
            double val;           
            if (chessBoard.IsGameOver())
            {
                return lostPenality - searchState.Depth;
            }
            if (searchState.Depth == 0)
            {
                val = Evaluator.EvalForMove(chessBoard);
                if (!searchState.OnMove)
                {
                    val = -val;
                }
                return val;
            }
            if (searchState.OnMove)
            {
                MoveSearch(searchState);
            }
            else
            {
                AddSearch(searchState);
            }
            return searchState.Alpha;
        }

        private void MoveSearch(SearchState searchState)
        {
            double val;
            searchState.TryBuildMoveStatus(chessBoard);
            ChessBoard[] boards = searchState.GetBoards();
            foreach (var board in boards)
            {
                BoardsCacheAlphaBetaAI newAI = new BoardsCacheAlphaBetaAI(board);
                SearchState newState = searchState.GetState(board);
                val = -newAI.AlphaBeta(newState);
                if (val > searchState.Alpha)
                {
                    searchState.Alpha = val;
                    searchState.BestDirection = searchState.GetDirection(board);
                }
                if (val >= searchState.Beta)
                {
                    return;
                }
            }
        }

        private void AddSearch(SearchState searchState)
        {
            searchState.TryBuildAddStatus(chessBoard);
            ChessBoard[] boards = searchState.GetBoards();
            foreach (var board in boards)
            {
                BoardsCacheAlphaBetaAI newAI = new BoardsCacheAlphaBetaAI(board);
                SearchState newState = searchState.GetState(board);
                double val = -newAI.AlphaBeta(newState);
                if (val > searchState.Alpha)
                {
                    searchState.Alpha = val;
                    searchState.BestPosition = searchState.GetPosition(board);
                }
                if (val >= searchState.Beta)
                {
                    return;
                }
            }
        }
    }
}
