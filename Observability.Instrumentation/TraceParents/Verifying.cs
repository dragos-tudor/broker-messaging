
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  static bool HasMinTraceParentParts(string[] parts, int minParts = 4) => parts.Length >= minParts;
}