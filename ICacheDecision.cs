namespace Project2048
{
    interface ICacheDecision<TDecision>
    {   
        TDecision BestDecision { get; set; }
    }
}
