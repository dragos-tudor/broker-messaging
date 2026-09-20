
namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  internal static async Task<(TData, TSignal, Exception?)>
    RunFastRetryAsync<TServices, TData, TSignal, TDecision>(
      TServices services,
      TData data,
      TDecision decision,
      Func<TDecision, TServices, TData, CancellationToken,
        Task<(TData, TSignal, Exception?)>> executeOperation,
      Func<TSignal, bool> canFastRetry,
      Func<TimeSpan, CancellationToken, Task<bool>> delayFastRetry,
      CancellationToken ct = default)
    where TServices : IFastRetryOptionsService
    {
      var options = services.GetFastRetryOptions();
      var retryCount = 0;

      while (true)
      {
        var result = await executeOperation(decision, services, data, ct);
        if (ct.IsCancellationRequested)
          return result;

        var (nextData, nextSignal, _) = result;
        if (!canFastRetry(nextSignal))
          return result;

        if (IsFastRetryExhausted(retryCount, options))
          return result;

        var delay = CalculateNextAttemptDelay(++retryCount, options);
        var isDelayed = await delayFastRetry(delay, ct);
        if (!isDelayed)
          return result;

        data = nextData;
      }
    }
}