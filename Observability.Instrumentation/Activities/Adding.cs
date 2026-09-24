
namespace Observability.Instrumentation;

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
    DateTimeOffset timeStamp,
    ActivityTagsCollection? tags = null) =>
      activity?.AddEvent(CreateActivityEvent(name, timeStamp, tags));
}