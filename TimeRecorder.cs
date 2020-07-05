using System;

namespace Project2048
{
    public class TimeRecorder : ITimeRecorder
    {
        private TimeSpan startTimeSpan;
        public TimeRecorder()
        {
            StartTimeRecord();
        }
        public double GetTotalMilliSeconds()
        {
            TimeSpan endTime = new TimeSpan(DateTime.Now.Ticks);
            return endTime.Subtract(startTimeSpan).TotalMilliseconds;
        }

        public double GetTotalSeconds()
        {

            TimeSpan endTime = new TimeSpan(DateTime.Now.Ticks);
            return endTime.Subtract(startTimeSpan).TotalSeconds;
        }

        public void StartTimeRecord()
        {
            startTimeSpan = new TimeSpan(DateTime.Now.Ticks);
        }
    }
}
