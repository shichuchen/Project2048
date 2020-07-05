using System;
using System.Collections.Generic;
using System.Linq;

namespace Project2048
{
    using Direction = Settings.Direction;
    public class AlphaBetaAI : IPlayer
    {
        private const double infinity = Evaluator.Infinity;
        public class HashItem
        {
            public HashItem(ChessBoard chessBoard)
            {
                this.chessBoard = chessBoard;
            }
            private ChessBoard chessBoard;

            public override int GetHashCode()
            {
                return chessBoard.GetHashCode();
            }
        }
        public class SearchState
        {
            public SearchState() { }
            private static readonly int searchMilliSecs = Settings.MaxSearchMilliSecs;
            private static readonly Direction[] directions = Settings.Directions;
            private const int minDepth = 6;
            private const Direction initBestDirection = Direction.None;
            private const Position initBestPosition = null;

            private static int TargetDepth = minDepth;
            public static int CutOff = 0;

            private CacheStatus<Direction, SearchState> directionStatus;
            private CacheStatus<Position, SearchState> positionStatus;
            private TimeRecorder timeRecorder;
            private bool VisitedThisState = false;
            private HashSet<ChessBoard> bitBoards = new HashSet<ChessBoard>();

            public int Depth { get; set; } = 1;
            public int Turn { get; set; } = 0;
            public double Alpha { get; set; } = -infinity / 2;
            public double Beta { get; set; } = infinity / 2;
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
                CutOff = 0;
                StartTimeRecord();
                InitDepth(chessBoard);
                bitBoards = new HashSet<ChessBoard>();
            }
            private void InitDepth(ChessBoard chessBoard)
            {
                chessBoard.CalculateValues();
                var distinctValues = chessBoard.IncludedValues.ToHashSet();
                int distinctCount = distinctValues.Count;
                TargetDepth = Math.Max(minDepth, (distinctCount - 2) * 2);
            }
            public void StartTimeRecord()
            {
                timeRecorder = new TimeRecorder();
            }
            public bool TimeOut()
            {
                return timeRecorder.GetTotalMilliSeconds() >= searchMilliSecs
                    && Depth >= TargetDepth;
            }
            public double GetSearchTime()
            {
                if (timeRecorder is null)
                {
                    throw new ArgumentNullException();
                }
                return timeRecorder.GetTotalMilliSeconds();
            }
            public void SetNextRound()
            {
                ++Depth;
                Alpha = -infinity;
                Beta = infinity;
            }
            public void BuildMoveStatus(ChessBoard chessBoard)
            {
                if (!VisitedThisState)
                {
                    VisitedThisState = true;
                    directionStatus = new CacheStatus<Direction, SearchState>(initBestDirection);
                    foreach (Direction direction in directions)
                    {
                        ChessBoard newBoard = chessBoard.Copy();
                        if (newBoard.Move(direction))
                        {
                            if (TryAddBitBoard(newBoard))
                            {
                                directionStatus.AddBoard(direction, newBoard);
                            }
                            else
                            {
                                CutOff++;
                            }
                        }
                    }
                }
            }
            public void BuildAddStatus(ChessBoard chessBoard)
            {
                if (!VisitedThisState)
                {
                    VisitedThisState = true;
                    Candidates candidates = new Candidates(chessBoard);
                    positionStatus = new CacheStatus<Position, SearchState>(initBestPosition);
                    foreach (int level in candidates.Levels)
                    {
                        foreach (Position position in candidates[level])
                        {
                            ChessBoard newBoard = chessBoard.Copy();
                            newBoard.AddNew(position, level);

                            if (TryAddBitBoard(newBoard))
                            {
                                positionStatus.AddBoard(position, newBoard);
                            }
                            else
                            {
                                CutOff++;
                            }
                        }
                    }
                }
            }
            private bool TryAddBitBoard(ChessBoard chessBoard)
            {
                // return true;
                var bitBoard = chessBoard;
                var transposeRight = chessBoard.ToTransposeRight();
                var transposeLeft = chessBoard.ToTransposeLeft();
                if (
                    !bitBoards.Contains(bitBoard)
                    && !bitBoards.Contains(transposeRight)
                    && !bitBoards.Contains(transposeLeft)
                    )
                {
                    bitBoards.Add(bitBoard);
                    return true;
                }
                return false;
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
                    var newState = positionStatus.GetStatus(chessBoard);
                    SetNextState(newState);
                    return newState;
                }
                else
                {
                    var newState = directionStatus.GetStatus(chessBoard);
                    SetNextState(newState);
                    return newState;
                }
            }
            private void SetNextState(SearchState newState)
            {
                newState.Alpha = -Beta;
                newState.Beta = -Alpha;
                newState.Turn = Turn + 1;
                newState.Depth = Depth - 1;
            }
        }
        public AlphaBetaAI(ChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }
        private readonly ChessBoard chessBoard;
        private Direction bestDirection = Direction.None;
        private static readonly bool printProcess = Settings.PrintProcess;
        public Direction GetMoveDirection()
        {
            SearchState searchState = new SearchState();
            searchState.Initialize(chessBoard);
            while (!searchState.TimeOut())
            {
                AlphaBeta(searchState);
                searchState.SetNextRound();
            }
            Analyser.StoreDepth(searchState.Depth);
            if (printProcess)
            {
                Console.WriteLine("\tDirection:\t{0}", bestDirection);
                Console.WriteLine("\tDepth:\t{0}", searchState.Depth);
                Console.WriteLine("\tCutOff:\t{0}", SearchState.CutOff);
                Console.WriteLine("\tSearchTime:\t{0}\n", searchState.GetSearchTime());
            }
            return bestDirection;
        }
        public Direction GetTestMoveDirection()
        {
            SearchState searchState = new SearchState();
            searchState.Initialize(chessBoard);
            while (!searchState.TimeOut())
            {
                AlphaBeta(searchState);
                Console.WriteLine("\tDirection:\t{0}", bestDirection);
                Console.WriteLine("\tDepth:\t{0}", searchState.Depth);
                searchState.SetNextRound();
            }
            Analyser.StoreDepth(searchState.Depth);
            if (printProcess)
            {
                Console.WriteLine("\tDirection:\t{0}", bestDirection);
                Console.WriteLine("\tDepth:\t{0}", searchState.Depth);
                Console.WriteLine("\tCutOff:\t{0}", SearchState.CutOff);
                Console.WriteLine("\tSearchTime:\t{0}\n", searchState.GetSearchTime());
            }
            return bestDirection;
        }
        public double AlphaBeta(SearchState searchState)
        {
            if (chessBoard.IsGameOver())
            {
                return Evaluator.LostPenality - searchState.Depth;
            }
            if (searchState.Turn % 2 == 0)
            {
                if (searchState.Depth == 0)
                {
                    return Evaluator.EvalForMove(chessBoard);
                }
                searchState.BuildMoveStatus(chessBoard);
                var boards = searchState.GetBoards();
                foreach (var board in boards)
                {
                    var newAI = new AlphaBetaAI(board);
                    var newState = searchState.GetState(board);
                    var val = -newAI.AlphaBeta(newState);
                    if (val >= searchState.Beta)
                    {
                        return searchState.Beta;
                    }
                    if (val > searchState.Alpha)
                    {
                        searchState.Alpha = val;
                        var direction = searchState.GetDirection(board);
                        bestDirection = direction;
                        searchState.BestDirection = direction;
                    }
                }
            }
            else
            {
                if (searchState.Depth == 0)
                {
                    return -Evaluator.EvalForMove(chessBoard);
                }
                searchState.BuildAddStatus(chessBoard);
                var boards = searchState.GetBoards();
                foreach (var board in boards)
                {
                    var newAI = new AlphaBetaAI(board);
                    var newState = searchState.GetState(board);
                    var val = -newAI.AlphaBeta(newState);
                    if (val >= searchState.Beta)
                    {
                        return searchState.Beta;
                    }
                    if (val > searchState.Alpha)
                    {
                        searchState.Alpha = val;
                        searchState.BestPosition = searchState.GetPosition(board);
                    }
                }
            }
            return searchState.Alpha;
        }

    }
}
