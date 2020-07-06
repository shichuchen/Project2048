using System;
using System.Collections.Generic;
using System.Linq;

namespace Project2048
{
    using Direction = Settings.Direction;
    using Board = UInt64;
    public class AlphaBetaAI : IPlayer
    {
        public AlphaBetaAI(ChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }
        private readonly ChessBoard chessBoard;
        private static readonly bool printProcess = Settings.PrintProcess;
        private const double infinity = Evaluator.Infinity;
        private const double lostPenality = -infinity / 3;
        private const double bound = infinity / 2;
        private const int hashExact = 0;
        private const int hashAlpha = 1;
        private const int hashBeta = 2;
        private static int cutOff = 0;
        private static readonly Direction[] directions = Settings.Directions;
        private static readonly int[] addLevels = ChessBoard.AddLevels;
        private static Dictionary<Board, HashItem> hashTable;
        public class HashItem
        {
            public int depth = 0;
            public int hashFlag = -1;
            public double value = 0;
        }
        public class SearchState
        {
            public SearchState() { }
            private static readonly int searchMilliSecs = Settings.MaxSearchMilliSecs;

            private const int minDepth = 6;
            private const Direction initBestDirection = Direction.None;
            private const Position initBestPosition = null;
            private static int TargetDepth = minDepth;

            private CacheStatus<Direction, SearchState> directionStatus;
            private CacheStatus<Position, SearchState> positionStatus;
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
                    directionStatus = new CacheStatus<Direction, SearchState>(initBestDirection);
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
                    if (CanAddBitBoard(newBoard))
                    {
                        directionStatus.AddBoard(direction, newBoard);
                    }
                }
            }

            public void TryBuildAddStatus(ChessBoard chessBoard)
            {
                if (!VisitedThisState)
                {
                    VisitedThisState = true;
                    Candidates candidates = Candidates.AllIn(chessBoard);
                    positionStatus = new CacheStatus<Position, SearchState>(initBestPosition);
                    foreach (int level in candidates.Levels)
                    {
                        foreach (Position position in candidates[level])
                        {
                            ChessBoard newBoard = chessBoard.Copy();
                            newBoard.AddNew(position, level);
                            TryAddPositionStatus(position, newBoard);
                        }
                    }
                }
            }

            private void TryAddPositionStatus(Position position, ChessBoard newBoard)
            {
                if (CanAddBitBoard(newBoard))
                {
                    positionStatus.AddBoard(position, newBoard);
                }
            }

            private bool CanAddBitBoard(ChessBoard chessBoard)
            {
                return true;
                //var bitBoard = chessBoard;
                //var transposeRight = chessBoard.ToTransposeRight();
                //var transposeLeft = chessBoard.ToTransposeLeft();
                //if (
                //    !bitBoards.Contains(bitBoard)
                //    && !bitBoards.Contains(transposeRight)
                //    && !bitBoards.Contains(transposeLeft)
                //    )
                //{
                //    bitBoards.Add(bitBoard);
                //    return true;
                //}
                //return false;
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

        private double ProbeHash(int depth, double alpha, double beta)
        {
            if (hashTable.TryGetValue(chessBoard.BitBoard, out var hashItem))
            {
                if (hashItem.depth >= depth)
                {

                    if (hashItem.hashFlag == hashExact)
                    {
                        return hashItem.value;
                    }
                    if ((hashItem.hashFlag == hashAlpha) && (hashItem.value <= alpha))
                    {
                        return alpha;
                    }
                    if ((hashItem.hashFlag == hashBeta) && (hashItem.value >= beta))
                    {
                        return beta;
                    }
                }
            }
            return infinity;
        }
        private void RecordHash(int depth, double value, int hashFlag)
        {
            if (hashTable.TryGetValue(chessBoard.BitBoard, out HashItem hashItem))
            {
                if (hashItem.depth > depth)
                {
                    return;
                }

            }
            hashTable[chessBoard.BitBoard] = new HashItem()
            {
                value = value,
                hashFlag = hashFlag,
                depth = depth,

            };
        }
        public Direction GetMoveDirection()
        {
            hashTable = new Dictionary<Board, HashItem>();
            cutOff = 0;
            SearchState searchState = new SearchState();
            searchState.Initialize(chessBoard);
            while (!searchState.TimeOut())
            {
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
                Console.WriteLine("\tDepth:\t{0}", searchState.Depth);
                Console.WriteLine("\tCutOff:\t{0}", cutOff);
                Console.WriteLine("\tSearchTime:\t{0}\n", searchState.GetSearchTime());
            }
        }
        public double AlphaBeta(SearchState searchState)
        {
            int hashFlag = hashAlpha;
            double val;
            if ((val = ProbeHash(searchState.Depth, searchState.Alpha, searchState.Beta)) != infinity)
            {
                ++cutOff;
                return val;
            }
            if (chessBoard.IsGameOver())
            {
                return lostPenality - searchState.Depth;
            }
            if(searchState.Depth == 0)
            {
                val = Evaluator.EvalForMove(chessBoard);
                if (!searchState.OnMove)
                {
                    val = -val;
                }
                RecordHash(searchState.Depth, val, hashExact);
                return val;
            }
            if (searchState.OnMove)
            {
                MoveSearch(searchState, ref hashFlag);
            }
            else
            {
                AddSearch(searchState, ref hashFlag);
            }
            RecordHash(searchState.Depth, searchState.Alpha, hashFlag);
            return searchState.Alpha;
        }

        private void MoveSearch(SearchState searchState, ref int hashFlag)
        {
            double val;
            if(hashTable.TryGetValue(chessBoard.BitBoard, out var hashItem))
            {

            }
            #region old version
            //searchState.TryBuildMoveStatus(chessBoard);
            //ChessBoard[] boards = searchState.GetBoards();
            //foreach (var board in boards)
            //{
            //    AlphaBetaAI newAI = new AlphaBetaAI(board);
            //    SearchState newState = searchState.GetState(board);
            //    val = -newAI.AlphaBeta(newState);
            //    if (val > searchState.Alpha)
            //    {
            //        hashFlag = hashExact;
            //        searchState.Alpha = val;
            //        searchState.BestDirection = searchState.GetDirection(board);
            //    }
            //    if (val >= searchState.Beta)
            //    {
            //        RecordHash(searchState.Depth, searchState.Beta, hashBeta);
            //        return;
            //    }
            //}
            #endregion
        }

        private void AddSearch(SearchState searchState, ref int hashFlag)
        {
            #region old version
            //searchState.TryBuildAddStatus(chessBoard);
            //ChessBoard[] boards = searchState.GetBoards();
            //foreach (var board in boards)
            //{
            //    AlphaBetaAI newAI = new AlphaBetaAI(board);
            //    SearchState newState = searchState.GetState(board);
            //    double val = -newAI.AlphaBeta(newState);
            //    if (val > searchState.Alpha)
            //    {
            //        hashFlag = hashExact;
            //        searchState.Alpha = val;
            //        searchState.BestPosition = searchState.GetPosition(board);
            //    }
            //    if (val >= searchState.Beta)
            //    {
            //        RecordHash(searchState.Depth, searchState.Beta, hashBeta);
            //        return;
            //    }
            #endregion
        }
    }
    }
}
