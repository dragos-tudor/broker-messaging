
namespace Observability.Instrumentation;

partial class InstrumentationFuncs
{
  static ActivityListener CreateActivityListener() =>
    new ()
    {
      ShouldListenTo = source => true,
      Sample = (ref options) => ActivitySamplingResult.AllDataAndRecorded
    };
}
