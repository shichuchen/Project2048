namespace Project2048
{
    internal interface ITimeRecorder
    {
        void StartTimeRecord();
        double GetTotalSeconds();
        double GetTotalMilliSeconds();
    }
}
