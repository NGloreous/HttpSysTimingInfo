namespace HttpSysTimingInfo
{
    public interface IHttpSysRequestTimingFeature
    {
        IEnumerable<long> Timestamps { get; }

        bool TryGetTimestamp(HttpSysRequestTimingType timingType, out long timestamp);

        bool TryGetElapsedTime(HttpSysRequestTimingType startTiming, HttpSysRequestTimingType endTiming, out TimeSpan elapsed);
    }
}
