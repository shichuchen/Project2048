using System;
using Project2048.Core;
using Project2048.Decorator;

namespace Project2048.Player
{
    using Direction = Settings.Direction;

    public class DecisionCacheAlphaBetaAi : ISearcher
    {
        public DecisionCacheAlphaBetaAi(ChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }

        private readonly ChessBoard chessBoard;
        private static readonly bool printProcess = Settings.printProcess;
        private static readonly Direction[] directions = Settings.Directions;
        private static readonly int[] levels = ChessBoard.AddLevels;
        private const double Infinity = Evaluator.infinity;
        private const double LostPenality = -Infinity / 3;
        private const double Bound = Infinity / 2;
        private static int cutOff;
        public int Depth { get; set; }

        public class SearchState
        {
            private static readonly int searchMilliSecs = Settings.maxSearchMilliSecs;
            private static TimeRecorder timeRecorder;
            private const int MinDepth = 6;
            private const Direction InitBestDirection = Direction.None;
            private const Position InitBestPosition = null;
            private static int targetDepth = MinDepth;
            private static readonly Position[] blankPositions = { };

            private DecisionKeyCacheStatus<Direction, SearchState> directionStatus;

            private DecisionKeyCacheStatus<Position, SearchState> levelOnePositionStatus;
            private DecisionKeyCacheStatus<Position, SearchState> levelTwoPositionStatus;

            private bool visited;

            public int Depth { get; set; } = 1;
            public int Turn { get; set; }
            public double Alpha { get; set; } = -Bound;
            public double Beta { get; set; } = Bound;
            public bool OnMove => (Turn & 1) == 0;

            public Direction BestDirection
            {
                get => directionStatus.BestDecision;
                set => directionStatus.BestDecision = value;
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
                targetDepth = Math.Max(MinDepth, (chessBoard.DistinctCount - 2) * 2);
            }
            public void StartTimeRecord()
            {
                timeRecorder = new TimeRecorder();
            }
            public bool TimeOut()
            {
                return
                    timeRecorder.GetTotalMilliSeconds() >= searchMilliSecs
                    && Depth >= targetDepth
                    ;
            }
            public double GetSearchTime()
            {
                if (timeRecorder is null)
                {
                    throw new NullReferenceException("Please initialize SearchState Before Searching");
                }
                return timeRecorder.GetTotalMilliSeconds();
            }
            public void ToNextRound()
            {
                ++Depth;
                Alpha = -Bound;
                Beta = Bound;
            }
            public void TryBuildMoveStatus(ChessBoard chessBoard)
            {
                if (!visited)
                {
                    visited = true;
                    directionStatus = new DecisionKeyCacheStatus<Direction, SearchState>(InitBestDirection);
                    foreach (var direction in directions)
                    {
                        var newBoard = chessBoard.Copy();
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
                    var candidates = InitializeByEmptyCount(chessBoard);
                    TryAddLevelOneStatus(candidates);
                    TryAddLevelTwoStatus(candidates);

                }
            }

            private void TryAddLevelTwoStatus(Candidates candidates)
            {
                foreach (var position in candidates[levels[1]])
                {
                    if (levelTwoPositionStatus is null)
                    {
                        levelTwoPositionStatus = new DecisionKeyCacheStatus<Position, SearchState>(InitBestPosition);
                    }
                    levelTwoPositionStatus.AddDecision(position);
                }
            }

            private void TryAddLevelOneStatus(Candidates candidates)
            {
                foreach (var position in candidates[levels[0]])
                {
                    if (levelOnePositionStatus is null)
                    {
                        levelOnePositionStatus = new DecisionKeyCacheStatus<Position, SearchState>(InitBestPosition);
                    }
                    levelOnePositionStatus.AddDecision(position);
                }
            }

            private static Candidates InitializeByEmptyCount(ChessBoard chessBoard)
            {
                int emptyCount = chessBoard.EmptyCount;
                var candidates = emptyCount > 2 ? Candidates.ChooseAnnoying(chessBoard) : Candidates.AllIn(chessBoard);
                return candidates;
            }

            public SearchState GetNextState(int level, Position position)
            {
                var newState = level == levels[0] ? levelOnePositionStatus.GetStatus(position) : levelTwoPositionStatus.GetStatus(position);
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
                var newState = directionStatus.GetStatus(direction);
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
            cutOff = 0;
            var searchState = new SearchState();
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
            Analyser.StoreCutOff(cutOff);
        }

        private void PrintProcess(SearchState searchState)
        {
            if (printProcess)
            {
                Console.WriteLine("\tDirection:\t{0}", searchState.BestDirection);
                Console.WriteLine("\tDepth:\t{0}", Depth);
                Console.WriteLine("\tCutOff:\t{0}", cutOff);
                Console.WriteLine("\tSearchTime:\t{0}\n", searchState.GetSearchTime());
            }
        }
        public double AlphaBeta(SearchState searchState)
        {

            if (chessBoard.IsGameOver())
            {
                return LostPenality - searchState.Depth;
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

            return -Evaluator.EvalForMove(chessBoard);
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
            searchState.TryBuildMoveStatus(chessBoard);
            var prevDirections = searchState.GetDirections();
            foreach (var direction in prevDirections)
            {
                var board = chessBoard.Copy();
                board.Move(direction);
                var newAi = new DecisionCacheAlphaBetaAi(board);
                var newStatus = searchState.GetNextState(direction);
                var val = -newAi.AlphaBeta(newStatus);
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
                var positions = searchState.GetPositions(level);
                foreach (var position in positions)
                {
                    chessBoard.AddNew(position, level);
                    var newState = searchState.GetNextState(level, position);
                    var newAi = new DecisionCacheAlphaBetaAi(chessBoard);
                    double val = -newAi.AlphaBeta(newState);
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



