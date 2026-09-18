
namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  internal static async Task<(TData, TSignal, Exception?)>
    ExecuteWithFastRetryAsync<TServices, TData, TSignal, TTransition>(
      TServices services,
      TData data,
      TTransition transition,
      Func<TTransition, TServices, TData, CancellationToken,
        Task<(TData, TSignal, Exception?)>> executeOperation,
      Func<TSignal, bool> canFastRetry,
      Func<TimeSpan, CancellationToken, Task<bool>> delayRetryExecution,
      CancellationToken ct = default)
    where TServices : IFastRetryOptionsService
    {
      var options = services.GetFastRetryOptions();
      var retryCount = 0;

      while (true)
      {
        var result = await executeOperation(transition, services, data, ct);
        if (ct.IsCancellationRequested)
          return result;

        var (nextData, nextSignal, _) = result;
        if (!canFastRetry(nextSignal))
          return result;

        if (IsRetryExecutionExhausted(retryCount, options))
          return result;

        var delay = CalculateNextAttemptDelay(++retryCount, options);
        var isDelayed = await delayRetryExecution(delay, ct);
        if (!isDelayed)
          return result;

        data = nextData;
      }
    }
}