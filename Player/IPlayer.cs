namespace Project2048
{
    using Direction = Settings.Direction;
    public interface IPlayer
    {
        Direction GetMoveDirection();
    }
}