namespace Project2048.Player
{
    internal interface ISearcher: IPlayer
    {   
        int Depth { get; set; }
    }
}
