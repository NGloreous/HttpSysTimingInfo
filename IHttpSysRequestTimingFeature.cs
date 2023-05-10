﻿namespace HttpSysTimingInfo
{
    public interface IHttpSysRequestTimingFeature
    {
        /// <summary>
        /// Gets all http.sys timing timestamps
        /// </summary>
        /// <remarks>
        /// These timestamps were originally retrieved using <see cref="System.Diagnostics.Stopwatch.GetTimestamp"/>.
        /// The index of the timing can be cast to <see cref="HttpSysRequestTimingType"/> to know what the timing represents.
        /// The value may be 0 if the timing is not available for the current request.
        /// </remarks>
        IEnumerable<long> Timestamps { get; }

        /// <summary>
        /// Gets the timestamp for the given timing.
        /// </summary>
        /// <remarks>
        /// The timestamp was originally retrieved using <see cref="System.Diagnostics.Stopwatch.GetTimestamp"/>.
        /// </remarks>
        /// <param name="timestampType">The timestamp type to get.</param>
        /// <param name="timestamp">The value of the timestamp if set.</param>
        /// <returns>True if the given timing was set (i.e., non-zero).</returns>
        bool TryGetTimestamp(HttpSysRequestTimingType timestampType, out long timestamp);

        /// <summary>
        /// Gets the elapsed time between the two given timings.
        /// </summary>
        /// <param name="startingTimestampType">The timestamp type marking the beginning of the time period.</param>
        /// <param name="endingTimestampType">The timestamp type marking the end of the time period.</param>
        /// <param name="elapsed">A <see cref="TimeSpan"/> for the elapsed time between the starting and ending timestamps.</param>
        /// <returns>True if both given timings were set (i.e., non-zero).</returns>
        bool TryGetElapsedTime(HttpSysRequestTimingType startingTimestampType, HttpSysRequestTimingType endingTimestampType, out TimeSpan elapsed);
    }
}