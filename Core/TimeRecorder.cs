using System;

namespace Project2048.Core
{
    public class TimeRecorder 
    {
        private TimeSpan startTimeSpan;
        public TimeRecorder()
        {
            StartTimeRecord();
        }
        public double GetTotalMilliSeconds()
        {
            var endTime = new TimeSpan(DateTime.Now.Ticks);
            return endTime.Subtract(startTimeSpan).TotalMilliseconds;
        }

        public double GetTotalSeconds()
        {

            var endTime = new TimeSpan(DateTime.Now.Ticks);
            return endTime.Subtract(startTimeSpan).TotalSeconds;
        }

        public void StartTimeRecord()
        {
            startTimeSpan = new TimeSpan(DateTime.Now.Ticks);
        }
    }
}
