using System;


namespace Project2048
{
    using Direction = Settings.Direction;

    internal class Human : IPlayer
    {
        public Direction GetMoveDirection()
        {
            char input = Console.ReadKey(true).KeyChar;
            input = char.ToLower(input);
            if (input == 'w')
            {
                return Direction.Up;
            }
            if (input == 's')
            {
                return Direction.Down;
            }
            if (input == 'a')
            {
                return Direction.Left;
            }
            if (input == 'd')
            {
                return Direction.Right;
            }
            return Direction.None;
        }
    }
}
