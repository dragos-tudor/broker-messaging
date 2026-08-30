#pragma warning disable CA2000

namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  internal const string System = "kafka.client";

  internal static Activity CreateActivity(
    ActivitySource activitySource,
    string activityName,
    ActivityKind activityKind,
    ActivityContext? activityContext = default) =>
      activityContext is not null ?
        EnsureActivitySourceListener(activitySource).StartActivity(activityName, activityKind, activityContext.Value)!:
        EnsureActivitySourceListener(activitySource).StartActivity(activityName, activityKind)!;

  internal static Activity CreateDefaultActivity(
    ActivitySource activitySource,
    string activityName,
    ActivityKind activityKind,
    string? component = default,
    string system = System) =>
      CreateActivity(activitySource, activityName, activityKind)
        .SetDefaultActivityTags(component ?? activityName, system);
}
