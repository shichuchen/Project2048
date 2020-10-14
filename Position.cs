namespace Project2048
{
    /// <summary>
    /// 定义棋子在棋盘中相对位置的类, 对于位棋盘, 表示移位值.
    /// </summary>
    public class Position : NamedInt
    {
        public Position(int value) : base(value)
        {
            Row = value / 16;
            Col = (value / 4) % 4;
        }
        public Position(int x, int y) :
            base(16 * x + 4 * y)
        {
            Row = x;
            Col = y;
        }
        public static implicit operator Position(int value) { return new Position(value); }
        public int Row { get; }
        public int Col { get; }
    }
}
