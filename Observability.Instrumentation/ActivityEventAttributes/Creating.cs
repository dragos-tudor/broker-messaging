
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  internal static KeyValuePair<string, object?> CreateActivityEventAttribute(string name, object? value)
    => new(name, value);
}