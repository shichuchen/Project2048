using Project2048.Core;

namespace Project2048.Player
{
    using Direction = Settings.Direction;
    public interface IPlayer
    {
        Direction GetMoveDirection();
    }
}