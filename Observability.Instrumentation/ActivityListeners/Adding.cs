
namespace Observability.Instrumentation;

partial class InstrumentationFuncs
{
  static ActivitySource AddActivityListener(
    ActivitySource source,
    ActivityListener listener)
  {
    ActivitySource.AddActivityListener(listener);
    return source;
  }
}
