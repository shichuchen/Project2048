using System;
using System.Collections.Generic;
using Project2048.Core;
using Project2048.Decorator;

namespace Project2048.Player
{
    using Direction = Settings.Direction;
    using Board = UInt64;
    public class HashCacheAlphaBetaAi : ISearcher
    {
        public HashCacheAlphaBetaAi(ChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }
        private readonly ChessBoard chessBoard;
        private static readonly bool printProcess = Settings.printProcess;
        private static readonly Direction[] directions = Settings.Directions;
        private const double Infinity = Evaluator.infinity;
        private const double LostPenality = -Infinity / 3;
        private const double Bound = Infinity / 2;
        private const int HashExact = 0;
        private const int HashAlpha = 1;
        private const int HashBeta = 2;
        private static int cutOff;
        public int Depth { get; set; }
        private static Dictionary<Board, HashItem> moveHashTable;
        private static Dictionary<Board, HashItem> addHashTable;

        public class HashItem
        {
            public int depth;
            public int hashFlag = -1;
            public double value;
            public Direction BestDirection;
            public Position BestPosition;
        }
        public class SearchState
        {
            private static readonly int searchMilliSecs = Settings.maxSearchMilliSecs;

            private const int MinDepth = 6;
            private const Direction InitBestDirection = Direction.None;
            private static  readonly Position initBestPosition = new Position();
            private static int targetDepth = MinDepth;

            private ChessBoadKeyCacheStatus<Direction, SearchState> directionStatus;
            private ChessBoadKeyCacheStatus<Position, SearchState> positionStatus;
            private TimeRecorder timeRecorder;
            private bool visitedThisState;

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
            public Position BestPosition
            {
                get => positionStatus.BestDecision;
                set => positionStatus.BestDecision = value;
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
                    throw new ArgumentNullException("Please initialize SearchState Before Searching");
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
                if (!visitedThisState)
                {
                    visitedThisState = true;
                    directionStatus = new ChessBoadKeyCacheStatus<Direction, SearchState>(InitBestDirection);
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
                    directionStatus.AddBoard(direction, newBoard);
                }
            }

            public void TryBuildAddStatus(ChessBoard chessBoard)
            {
                if (!visitedThisState)
                {
                    visitedThisState = true;
                    var candidates = Candidates.AllIn(chessBoard);
                    positionStatus = new ChessBoadKeyCacheStatus<Position, SearchState>(initBestPosition);
                    foreach (int level in candidates.Levels)
                    {
                        foreach (var position in candidates[level])
                        {
                            var newBoard = chessBoard.Copy();
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
            public void SetNextState(SearchState newState)
            {
                newState.Alpha = -Beta;
                newState.Beta = -Alpha;
                newState.Turn = Turn + 1;
                newState.Depth = Depth - 1;
            }
            public double ProbeMoveHashTable(ChessBoard chessBoard)
            {
                if (moveHashTable.TryGetValue(chessBoard.BitBoard, out var hashItem))
                {
                    if (hashItem.depth >= Depth)
                    {

                        if (hashItem.hashFlag == HashExact)
                        {
                            return hashItem.value;
                        }
                        if ((hashItem.hashFlag == HashAlpha) && (hashItem.value <= Alpha))
                        {
                            return Alpha;
                        }
                        if ((hashItem.hashFlag == HashBeta) && (hashItem.value >= Beta))
                        {
                            return Beta;
                        }
                    }
                }
                return Infinity;
            }
            public double ProbeAddHashTable(ChessBoard chessBoard)
            {
                if (addHashTable.TryGetValue(chessBoard.BitBoard, out var hashItem))
                {
                    if (hashItem.depth >= Depth)
                    {

                        if (hashItem.hashFlag == HashExact)
                        {
                            return hashItem.value;
                        }
                        if ((hashItem.hashFlag == HashAlpha) && (hashItem.value <= Alpha))
                        {
                            return Alpha;
                        }
                        if ((hashItem.hashFlag == HashBeta) && (hashItem.value >= Beta))
                        {
                            return Beta;
                        }
                    }
                }
                return Infinity;
            }
        }
        private void RecordMoveHash(int depth, double value, int hashFlag)
        {
            if (moveHashTable.TryGetValue(chessBoard.BitBoard, out var hashItem))
            {
                if (hashItem.depth > depth)
                {
                    return;
                }

            }
            moveHashTable[chessBoard.BitBoard] = new HashItem()
            {
                value = value,
                hashFlag = hashFlag,
                depth = depth,

            };
        }
        private void RecordAddHash(int depth, double value, int hashFlag)
        {
            if (addHashTable.TryGetValue(chessBoard.BitBoard, out var hashItem))
            {
                if (hashItem.depth > depth)
                {
                    return;
                }

            }
            addHashTable[chessBoard.BitBoard] = new HashItem()
            {
                value = value,
                hashFlag = hashFlag,
                depth = depth,

            };
        }
        public Direction GetMoveDirection()
        {
            Initialize();
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

        private static void Initialize()
        {
            moveHashTable = new Dictionary<Board, HashItem>();
            addHashTable = new Dictionary<Board, HashItem>();
            cutOff = 0;
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
            double val = ProbeHashTable(searchState);
            if (val != Infinity)
            {
                ++cutOff;
                return val;
            }
            if (chessBoard.IsGameOver())
            {
                return LostPenality - searchState.Depth;
            }
            if (searchState.Depth == 0)
            {
                return EvalAndRecord(searchState);
            }
            int hashFlag = Search(searchState);
            RecordHashTable(searchState, hashFlag);
            return searchState.Alpha;
        }

        private int Search(SearchState searchState)
        {
            int hashFlag = HashAlpha;
            if (searchState.OnMove)
            {
                MoveSearch(searchState, ref hashFlag);
            }
            else
            {
                AddSearch(searchState, ref hashFlag);
            }

            return hashFlag;
        }

        private void RecordHashTable(SearchState searchState, int hashFlag)
        {
            if (searchState.OnMove)
            {
                RecordMoveHash(searchState.Depth, searchState.Alpha, hashFlag);
            }
            else
            {
                RecordAddHash(searchState.Depth, searchState.Alpha, hashFlag);
            }
        }

        private double EvalAndRecord(SearchState searchState)
        {
            double val;
            if (searchState.OnMove)
            {
                val = Evaluator.EvalForMove(chessBoard);
                RecordMoveHash(searchState.Depth, val, HashExact);
            }
            else
            {
                val = -Evaluator.EvalForMove(chessBoard);
                RecordAddHash(searchState.Depth, val, HashExact);
            }
            return val;
        }

        private double ProbeHashTable(SearchState searchState)
        {
            var val = searchState.OnMove ? searchState.ProbeMoveHashTable(chessBoard) : searchState.ProbeAddHashTable(chessBoard);

            return val;
        }

        private void MoveSearch(SearchState searchState, ref int hashFlag)
        {
            searchState.TryBuildMoveStatus(chessBoard);
            var boards = searchState.GetBoards();
            foreach (var board in boards)
            {
                var newAi = new HashCacheAlphaBetaAi(board);
                var newState = searchState.GetState(board);
                var val = -newAi.AlphaBeta(newState);
                if (val > searchState.Alpha)
                {
                    hashFlag = HashExact;
                    searchState.Alpha = val;
                    searchState.BestDirection = searchState.GetDirection(board);
                }
                if (val >= searchState.Beta)
                {
                    RecordMoveHash(searchState.Depth, searchState.Beta, HashBeta);
                    return;
                }
            }
        }

        private void AddSearch(SearchState searchState, ref int hashFlag)
        {
            searchState.TryBuildAddStatus(chessBoard);
            var boards = searchState.GetBoards();
            foreach (var board in boards)
            {
                var newAi = new HashCacheAlphaBetaAi(board);
                var newState = searchState.GetState(board);
                double val = -newAi.AlphaBeta(newState);
                if (val > searchState.Alpha)
                {
                    hashFlag = HashExact;
                    searchState.Alpha = val;
                    searchState.BestPosition = searchState.GetPosition(board);
                }
                if (val >= searchState.Beta)
                {
                    RecordAddHash(searchState.Depth, searchState.Beta, HashBeta);
                    return;
                }
            }
        }
    }
}
