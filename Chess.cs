namespace Project2048
{
    public class Chess
    {
        public Chess(Position position, int level)
        {
            Position = position;
            Level = level;
        }
        public Position Position { get; }
        public int Level
        {
            get { return level; }
            set
            {
                level = value;
                if (level >= 1)
                {
                    Value = Values[level - 1];
                }
                else
                {
                    Value = 0;
                }
            }
        }
        public int Value { get; private set; } = 0;
        public int Row { get { return Position.Row; } }
        public int Col { get { return Position.Col; } }
        public const int EmptyValue = 0;

        private int level = 0;
        public static readonly int[] Values =
        {
            2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768

        };
        public static readonly int[] AddLevels =
        {
            1, 2
        };
    }
}
