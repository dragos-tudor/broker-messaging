
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  internal static void AddMetricCounter(Counter<long> counters, long delta = 1) => counters.Add(delta);
}