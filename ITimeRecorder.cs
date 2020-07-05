namespace Project2048
{
    interface ITimeRecorder
    {
        void StartTimeRecord();
        double GetTotalSeconds();
        double GetTotalMilliSeconds();
    }
}
