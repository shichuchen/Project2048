namespace Project2048
{
    public class Settings
    {
        public enum Direction : int
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
            None = 4,
        }
        public static readonly Direction[] Directions =
        {
            Direction.Up, Direction.Down, Direction.Left, Direction.Right,
        };

        public const int MaxSearchMilliSecs = 100;

        public const int MaxRound = 100;
        public const bool PrintProcess = false;
        public const bool OnAnalyse = true;
        public const bool OnEvolve = false;
    }
}
