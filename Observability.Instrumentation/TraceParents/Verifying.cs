
namespace Observability.Instrumentation;

partial class InstrumentationFuncs
{
  public static bool IsTraceParentW3C(Activity activity) =>
    activity.IdFormat == ActivityIdFormat.W3C;
}
