
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  public static string? ToTraceParent(Activity activity) =>
    $"00-{activity.TraceId}-{activity.SpanId}-{(byte)activity.ActivityTraceFlags:x2}";
}