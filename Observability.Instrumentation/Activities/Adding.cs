
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  internal static Activity? AddActivityTag(
    Activity? activity,
    string key,
    object? value) =>
      activity?.AddTag(key, value);

  internal static Activity? AddActivityEvent(
    Activity? activity,
    string name,
    IEnumerable<KeyValuePair<string, object?>>? attributes = null,
    DateTimeOffset? timeStamp = default) =>
      activity?.AddEvent(CreateActivityEvent(name, timeStamp, attributes));
}