
namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  const string SystemActivityKey = "kafka.system";
  const string ComponentActivityKey = "kafka.component";

  internal static Activity SetDefaultActivityTags(
    this Activity activity,
    string component,
    string system) =>
      activity
        .AddTag(SystemActivityKey, system)
        .AddTag(ComponentActivityKey, component);

  internal static Activity? SetActivityParentId(
    Activity? activity,
    ActivityContext activityContext) =>
      activity?.SetParentId(activityContext.TraceId, activityContext.SpanId, activityContext.TraceFlags);

}