
namespace Observability.Instrumentation;

partial class InstrumentationFuncs
{
  internal static ActivityContext? ToActivityContext(string? traceParent) =>
    ActivityContext.TryParse(traceParent, default, out ActivityContext context)?
      context: default;
}