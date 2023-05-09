using HttpSysTimingInfo;
using Microsoft.AspNetCore.Server.HttpSys;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Debug.Assert(OperatingSystem.IsWindows());
builder.WebHost.UseHttpSys();

var app = builder.Build();

app.MapGet("/", context =>
{
    var received = TimeProvider.System.GetTimestamp();

    var requestInfoFeature = context.Features.Get<IHttpSysRequestInfoFeature>() ?? throw new NotSupportedException();
    var requestTimingInfo = new HttpSysRequestTimingFeature(requestInfoFeature);

    var sb = new StringBuilder();
    if (requestTimingInfo.TryGetElapsedTime(HttpSysRequestTimingType.RequestHeaderParseStart, HttpSysRequestTimingType.RequestHeaderParseEnd, out var headerParse))
    {
        sb.AppendLine($"Header Parse: {headerParse}");
    }

    if (requestTimingInfo.TryGetElapsedTime(HttpSysRequestTimingType.RequestQueuedForIO, HttpSysRequestTimingType.RequestDeliveredForIO, out var queued))
    {
        sb.AppendLine($"Queued for IO: {queued}");
    }

    if (requestTimingInfo.TryGetElapsedTime(HttpSysRequestTimingType.RequestHeaderParseStart, HttpSysRequestTimingType.RequestDeliveredForIO, out var overall))
    {
        sb.AppendLine($"Htty.sys overall: {overall}");
    }

    if (requestTimingInfo.TryGetTimestamp(HttpSysRequestTimingType.RequestDeliveredForIO, out long delivered))
    {
        var aspNetOverhead = TimeProvider.System.GetElapsedTime(delivered, received);
        sb.AppendLine($"Asp.net overhead: {aspNetOverhead}");
    }

    return context.Response.WriteAsync(sb.ToString());
});
app.MapGet("/dump", context =>
{
    var requestInfoFeature = context.Features.Get<IHttpSysRequestInfoFeature>() ?? throw new NotSupportedException();
    var requestTimingInfo = new HttpSysRequestTimingFeature(requestInfoFeature);

    var timings = requestTimingInfo.Timestamps
        .Select((t, i) => $"{Enum.GetName((HttpSysRequestTimingType)i)}:\t{t}");

    return context.Response.WriteAsync($"Current:\t{Stopwatch.GetTimestamp()}" + Environment.NewLine + string.Join(Environment.NewLine, timings));
});

app.Run();
