namespace Project2048
{
    internal interface ICacheDecision<TDecision>
    {   
        TDecision BestDecision { get; set; }
    }
}
