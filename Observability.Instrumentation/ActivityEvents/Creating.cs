
namespace Observability.Instrumentation;

partial class InstrumentationFuncs
{
  static ActivityEvent CreateActivityEvent(
    string name,
    DateTimeOffset timeStamp,
    ActivityTagsCollection? tags = default) =>
      new(name, timeStamp, tags);
}