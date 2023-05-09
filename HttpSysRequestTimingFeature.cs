namespace HttpSysTimingInfo
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.AspNetCore.Server.HttpSys;

    public class HttpSysRequestTimingFeature : IHttpSysRequestTimingFeature
    {
        private readonly long[] timestamps;

        public HttpSysRequestTimingFeature(IHttpSysRequestInfoFeature httpSysRequestInfo)
        {
            /*
                Below is the definition of the timing info structure we are accessing the memory for.
                We can skip the first ULONG since it's always set to HttpRequestTimingTypeMax which is the size of the array.

                TODO: I would expect 244 bytes (4 bytes + 8 bytes * 30 [current size of HttpRequestTimingTypeMax]) but it's 248.
                It seems the RequestTimingCount is stored in 8 bytes as a ULONGLONG but I'm not sure why yet.

                typedef struct _HTTP_REQUEST_TIMING_INFO
                {
                    ULONG RequestTimingCount;
                    ULONGLONG RequestTiming[HttpRequestTimingTypeMax];

                } HTTP_REQUEST_TIMING_INFO, *PHTTP_REQUEST_TIMING_INFO;
             */
            this.timestamps = MemoryMarshal.Cast<byte, long>(
                httpSysRequestInfo.RequestInfo[(int)HTTP_REQUEST_INFO_TYPE.HttpRequestInfoTypeRequestTiming].Span.Slice(sizeof(long))).ToArray();
        }

        public IEnumerable<long> Timestamps => this.timestamps;

        public bool TryGetElapsedTime(HttpSysRequestTimingType startTiming, HttpSysRequestTimingType endTiming, out TimeSpan elapsed)
        {
            if (startTiming > endTiming)
            {
                throw new ArgumentException("Start timing should be less than end timing");
            }

            if (this.TryGetTimestamp(startTiming, out long startTimestamp) && this.TryGetTimestamp(endTiming, out long endTimestamp))
            {
                elapsed = TimeProvider.System.GetElapsedTime(startTimestamp, endTimestamp);
                return true;
            }

            elapsed = default;
            return false;
        }

        public bool TryGetTimestamp(HttpSysRequestTimingType timingType, out long timestamp)
        {
            int index = (int)timingType;
            if (index < this.timestamps.Length && this.timestamps[index] > 0)
            {
                timestamp = this.timestamps[index];
                return true;
            }

            timestamp = default;
            return false;
        }
    }

    public enum HTTP_REQUEST_INFO_TYPE
    {
        HttpRequestInfoTypeAuth,
        HttpRequestInfoTypeChannelBind,
        HttpRequestInfoTypeSslProtocol,
        HttpRequestInfoTypeSslTokenBindingDraft,
        HttpRequestInfoTypeSslTokenBinding,
        HttpRequestInfoTypeRequestTiming,
        HttpRequestInfoTypeTcpInfoV0,
        HttpRequestInfoTypeRequestSizing,
        HttpRequestInfoTypeQuicStats,
        HttpRequestInfoTypeTcpInfoV1
    }
}
