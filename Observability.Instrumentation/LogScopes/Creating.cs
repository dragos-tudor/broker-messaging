
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  internal static IDisposable CreateComponentLogScope(
    ILogger logger,
    Activity? activity,
    string component)
  {
    var scope = new Dictionary<string, object?>
    {
      ["traceId"] = activity?.TraceId.ToString(),
      ["spanId"] = activity?.SpanId.ToString(),
      ["component"] = component
    };
    return logger.BeginScope(scope)!;
  }
}