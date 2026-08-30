
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  static string[] SplitTraceParent(string traceParent) =>
    traceParent.Split('-', StringSplitOptions.RemoveEmptyEntries);
}