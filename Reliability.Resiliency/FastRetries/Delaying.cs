
namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  internal static async Task<bool> DelayRetryExecutionAsync(
    TimeSpan delay,
    CancellationToken ct = default)
  {
    try
    {
      await Task.Delay(delay, ct);
      return true;
    }
    catch (OperationCanceledException)
    {
      return false;
    }
  }
}