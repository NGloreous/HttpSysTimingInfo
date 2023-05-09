namespace HttpSysTimingInfo
{
    using System.Diagnostics;

    /// <summary>
    /// Being lazy so I don't have to try to reference .NET 8 preview and just copying what I need here. 
    /// </summary>
    public class TimeProvider
    {
        public static TimeProvider System { get; } = new SystemTimeProvider();

        public virtual long TimestampFrequency => Stopwatch.Frequency;

        public virtual long GetTimestamp() => Stopwatch.GetTimestamp();

        public TimeSpan GetElapsedTime(long startingTimestamp, long endingTimestamp)
        {
            long timestampFrequency = TimestampFrequency;
            if (timestampFrequency <= 0)
            {
                throw new InvalidOperationException();
            }

            return new TimeSpan((long)((endingTimestamp - startingTimestamp) * ((double)TimeSpan.TicksPerSecond / timestampFrequency)));
        }

        private sealed class SystemTimeProvider : TimeProvider
        {
            internal SystemTimeProvider() : base()
            {
            }
        }
    }
}
