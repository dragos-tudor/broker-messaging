
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  static string ToTraceParent(Activity activity) =>
    $"00-{activity.TraceId}-{activity.SpanId}-{ToTraceParentTraceFlags(activity)}";

  static string ToTraceParentTraceFlags(Activity activity) =>
    activity.Recorded ? RecordedTraceFlags : NoneTraceFlags;
}