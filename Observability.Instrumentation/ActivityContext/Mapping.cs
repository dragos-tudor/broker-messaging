
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  internal static ActivityContext? ToActivityContext(string? traceParent)
  {
    if (string.IsNullOrEmpty(traceParent)) return null;

    try {
      var parts = SplitTraceParent(traceParent);
      if (!HasMinTraceParentParts(parts)) return null;

      return new ActivityContext(
        ActivityTraceId.CreateFromString(parts[1].AsSpan()),
        ActivitySpanId.CreateFromString(parts[2].AsSpan()),
        GetActivityTraceFlags(parts[3])
      );
    }
    catch { return null; }
  }
}