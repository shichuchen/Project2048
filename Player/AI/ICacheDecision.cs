namespace Project2048.Player
{
    internal interface ICacheDecision<TDecision>
    {   
        TDecision BestDecision { get; set; }
    }
}
