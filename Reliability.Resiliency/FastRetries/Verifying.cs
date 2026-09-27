
namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  internal static async Task<bool> IsFastRetryDelayedAsync(
    int retryCount,
    FastRetryOptions options,
    CancellationToken ct = default)
  {
    if (ct.IsCancellationRequested) return false;
    if (IsFastRetryExhausted(retryCount, options)) return false;

    var delay = CalculateNextAttemptDelay(retryCount + 1, options);
    return await DelayFastRetryAsync(delay, ct);
  }

  static bool IsFastRetryExhausted(int retryCount, FastRetryOptions options) =>
    retryCount >= options.MaxRetryAttempts;
}