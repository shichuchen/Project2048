using System;

namespace Project2048.Core
{
    /// <summary>
    /// Represent the shift value of position
    /// </summary>
    public readonly struct Position : IEquatable<Position>
    {
        public Position(int value)
        {
            Value = value;
            Row = value / 16;
            Col = (value / 4) % 4;
        }
        public Position(int x, int y)
        {
            Value = 16 * x + 4 * y;
            Row = x;
            Col = y;
        }
        private int Value { get; }
        public int Row { get; }
        public int Col { get; }
        public static implicit operator Position(int value) => new Position(value);
        public static implicit operator int(Position pos) => pos.Value;
        public static bool operator ==(Position lhs, Position rhs) => lhs.Equals(rhs);

        public static bool operator !=(Position lhs, Position rhs) => !(lhs == rhs);
        public bool Equals(Position other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Position other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public override string ToString()
        {
            return $"({Row}, {Col})";
        }
    }
}
