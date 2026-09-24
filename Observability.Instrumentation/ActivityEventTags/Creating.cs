
namespace Observability.Instrumentation;

partial class InstrumentationFuncs
{
  internal static KeyValuePair<string, object?> CreateActivityEventTag(
    string name,
    object? value) =>
      new(name, value);
}