
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  static ActivityEvent CreateActivityEvent(
    string name,
    DateTimeOffset? timeStamp = default,
    IEnumerable<KeyValuePair<string, object?>>? attributes = null)
  =>
    new(
      name,
      timeStamp ?? DateTimeOffset.UtcNow,
      attributes is null ? null : [.. attributes]
    );
}