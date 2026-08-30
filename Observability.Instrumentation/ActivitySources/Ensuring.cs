#pragma warning disable CA2000

namespace ObservabilityInstrumentation;

partial class InstrumentationFuncs
{
  static ActivitySource EnsureActivitySourceListener(ActivitySource activitySource)
  {
    if (activitySource.HasListeners()) return activitySource;

    ActivitySource.AddActivityListener(new ActivityListener
    {
      ShouldListenTo = source => true,
      Sample = (ref options) =>
        ActivitySamplingResult.AllDataAndRecorded
    });
    return activitySource;
  }
}
