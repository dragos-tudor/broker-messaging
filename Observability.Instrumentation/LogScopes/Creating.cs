
namespace Observability.Instrumentation;

partial class InstrumentationFuncs
{
  const string TraceIdScope = "traceId";
  const string SpanIdScope = "spanId";
  const string ComponentScope = "component";

  internal static IDisposable CreateLogScope(
    ILogger logger,
    Activity? activity,
    string? component = default)
  {
    var scopes = new Dictionary<string, object?>
    {
      [TraceIdScope] = activity?.TraceId.ToString(),
      [SpanIdScope] = activity?.SpanId.ToString(),
      [ComponentScope] = component
    };
    return logger.BeginScope(scopes)!;
  }
}