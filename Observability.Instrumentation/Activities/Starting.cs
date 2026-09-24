#pragma warning disable CA2000

namespace Observability.Instrumentation;

partial class InstrumentationFuncs
{
  const string System = "messaging";

  internal static Activity? StartActivity(
    ActivitySource source,
    string name,
    ActivityKind kind,
    ActivityContext? context = default) =>
      (context is not null, source.HasListeners()) switch {
        (true, false) => AddActivityListener(source, CreateActivityListener()).StartActivity(name, kind, context!.Value),
        (false, false) => AddActivityListener(source, CreateActivityListener()).StartActivity(name, kind),
        (true, true) => source.StartActivity(name, kind, context!.Value),
        (false, true) => source.StartActivity(name, kind)
      };

  internal static Activity? StartSystemActivity(
    ActivitySource source,
    string name,
    ActivityKind kind) =>
      StartActivity(source, name, kind)?
        .SetSystemActivityTags(System);
}
