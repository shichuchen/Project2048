using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2048
{
    using Direction = Settings.Direction;
    using Board = UInt64;
    public class DecisionCacheAlphaBetaAI : ISearcher
    {
        public DecisionCacheAlphaBetaAI(ChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }

        private readonly ChessBoard chessBoard;
        private static readonly bool printProcess = Settings.PrintProcess;
        private static readonly Direction[] directions = Settings.Directions;
        private static readonly int[] levels = ChessBoard.AddLevels;
        private const double infinity = Evaluator.Infinity;
        private const double lostPenality = -infinity / 3;
        private const double bound = infinity / 2;
        private static int CutOff = 0;
        public int Depth { get; set; }

        public class SearchState
        {
            public SearchState() { }
            private static readonly int searchMilliSecs = Settings.MaxSearchMilliSecs;
            private static TimeRecorder timeRecorder;
            private const int minDepth = 6;
            private const Direction initBestDirection = Direction.None;
            private const Position initBestPosition = null;
            private static int TargetDepth = minDepth;
            private static readonly Position[] blankPositions = new Position[] { };

            private DecisionKeyCacheStatus<Direction, SearchState> directionStatus;

            private DecisionKeyCacheStatus<Position, SearchState> levelOnePositionStatus;
            private DecisionKeyCacheStatus<Position, SearchState> levelTwoPositionStatus;

            private bool visited = false;

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
            public void SetBestPosition(int level, Position position)
            {
                if (level == levels[0])
                {
                    levelOnePositionStatus.BestDecision = position;
                }
                else
                {
                    levelTwoPositionStatus.BestDecision = position;
                }
            }
            public void Initialize(ChessBoard chessBoard)
            {
                StartTimeRecord();
                InitDepth(chessBoard);
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
                    && Depth >= TargetDepth
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
                if (!visited)
                {
                    visited = true;
                    directionStatus = new DecisionKeyCacheStatus<Direction, SearchState>(initBestDirection);
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
                    directionStatus.AddDecision(direction);
                }
            }

            public void TryBuildAddStatus(ChessBoard chessBoard)
            {
                if (!visited)
                {
                    visited = true;
                    Candidates candidates = InitializeByEmptyCount(chessBoard);
                    TryAddLevelOneStatus(candidates);
                    TryAddLevelTwoStatus(candidates);

                }
            }

            private void TryAddLevelTwoStatus(Candidates candidates)
            {
                foreach (Position position in candidates[levels[1]])
                {
                    if (levelTwoPositionStatus is null)
                    {
                        levelTwoPositionStatus = new DecisionKeyCacheStatus<Position, SearchState>(initBestPosition);
                    }
                    levelTwoPositionStatus.AddDecision(position);
                }
            }

            private void TryAddLevelOneStatus(Candidates candidates)
            {
                foreach (Position position in candidates[levels[0]])
                {
                    if (levelOnePositionStatus is null)
                    {
                        levelOnePositionStatus = new DecisionKeyCacheStatus<Position, SearchState>(initBestPosition);
                    }
                    levelOnePositionStatus.AddDecision(position);
                }
            }

            private static Candidates InitializeByEmptyCount(ChessBoard chessBoard)
            {
                int emptyCount = chessBoard.EmptyCount;
                Candidates candidates;
                if (emptyCount > 2)
                {
                    candidates = Candidates.ChooseAnnoying(chessBoard);
                }
                else
                {
                    candidates = Candidates.AllIn(chessBoard);
                }
                return candidates;
            }

            public SearchState GetNextState(int level, Position position)
            {
                SearchState newState;
                if (level == levels[0])
                {
                    newState = levelOnePositionStatus.GetStatus(position);
                }
                else
                {
                    newState = levelTwoPositionStatus.GetStatus(position);
                }
                SetNextState(newState);
                return newState;
            }
            public Position[] GetPositions(int level)
            {
                if (level == levels[0])
                {
                    if (levelOnePositionStatus is null)
                    {
                        return blankPositions;
                    }
                    return levelOnePositionStatus.GetDecisions();
                }
                else
                {
                    if (levelTwoPositionStatus is null)
                    {
                        return blankPositions;
                    }
                    return levelTwoPositionStatus.GetDecisions();
                }
            }

            public Direction[] GetDirections()
            {
                return directionStatus.GetDecisions();
            }
            public SearchState GetNextState(Direction direction)
            {
                SearchState newState = directionStatus.GetStatus(direction);
                SetNextState(newState);
                return newState;

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

            if (chessBoard.IsGameOver())
            {
                return lostPenality - searchState.Depth;
            }
            if (searchState.Depth == 0)
            {
                return Eval(searchState);
            }

            Search(searchState);

            return searchState.Alpha;
        }
        private double Eval(SearchState searchState)
        {
            if (searchState.OnMove)
            {
                return Evaluator.EvalForMove(chessBoard);
            }
            else
            {
                return -Evaluator.EvalForMove(chessBoard);
            }
        }

        private void Search(SearchState searchState)
        {
            if (searchState.OnMove)
            {
                MoveSearch(searchState);
            }
            else
            {
                AddSearch(searchState);
            }
        }

        private void MoveSearch(SearchState searchState)
        {
            double val;
            searchState.TryBuildMoveStatus(chessBoard);
            Direction[] directions = searchState.GetDirections();
            foreach (Direction direction in directions)
            {
                ChessBoard board = chessBoard.Copy();
                board.Move(direction);
                DecisionCacheAlphaBetaAI newAI = new DecisionCacheAlphaBetaAI(board);
                SearchState newStatus = searchState.GetNextState(direction);
                val = -newAI.AlphaBeta(newStatus);
                if (val > searchState.Alpha)
                {
                    searchState.Alpha = val;
                    searchState.BestDirection = direction;
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
            foreach (int level in levels)
            {
                Position[] positions = searchState.GetPositions(level);
                foreach (Position position in positions)
                {
                    chessBoard.AddNew(position, level);
                    SearchState newState = searchState.GetNextState(level, position);
                    DecisionCacheAlphaBetaAI newAI = new DecisionCacheAlphaBetaAI(chessBoard);
                    double val = -newAI.AlphaBeta(newState);
                    chessBoard.SetEmpty(position);
                    if (val > searchState.Alpha)
                    {
                        searchState.Alpha = val;
                        searchState.SetBestPosition(level, position);
                    }
                    if (val >= searchState.Beta)
                    {
                        return;
                    }
                }
            }
        }
    }
}



