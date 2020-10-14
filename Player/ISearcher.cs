namespace Project2048
{
    internal interface ISearcher: IPlayer
    {   
        int Depth { get; set; }
    }
}
