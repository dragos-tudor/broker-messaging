
namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  internal static Task DelayTask(TimeSpan delay, CancellationToken cancellationToken)
    => Task.Delay(delay, cancellationToken);
}