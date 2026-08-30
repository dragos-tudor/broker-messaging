
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  const string RecordedTraceFlags = "01";
  const string NoneTraceFlags = "00";

  static ActivityTraceFlags GetActivityTraceFlags(string? flags) =>
    !string.IsNullOrEmpty(flags) && flags == RecordedTraceFlags ?
      ActivityTraceFlags.Recorded :
      ActivityTraceFlags.None;
}