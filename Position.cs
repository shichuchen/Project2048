﻿namespace Project2048
{
    /// <summary>
    /// Core class 
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
        public int Row { get; private set; }
        public int Col { get; private set; }
    }
}