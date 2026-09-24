
namespace Observability.Instrumentation;

partial class InstrumentationFuncs
{
  const string SystemTagName = "messaging";

  internal static Activity SetSystemActivityTags(
    this Activity activity,
    string system) =>
      activity.AddTag(
        SystemTagName,
        system
      );

  internal static Activity? SetActivityParentId(
    Activity? activity,
    ActivityContext context) =>
      activity?.SetParentId(
        context.TraceId,
        context.SpanId,
        context.TraceFlags
      );

}