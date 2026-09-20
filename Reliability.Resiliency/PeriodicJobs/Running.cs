
namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  static readonly TimeSpan SafeClosingInterval = TimeSpan.FromSeconds(3);

  internal static async Task RunPeriodicJobAsync(
    string jobName,
    TimeSpan timerInterval,
    TimeSpan lockInterval,
    Func<CancellationToken, Task> work,
    IPeriodicJobServices services,
    CancellationToken ct = default)
  {
    using var timer = new PeriodicTimer(timerInterval);
    while (!ct.IsCancellationRequested)
    {
      try
      {
        if(!await timer.WaitForNextTickAsync(ct))
          return;

        var workTimeout = lockInterval - SafeClosingInterval;

        using var workCts = new CancellationTokenSource(workTimeout);
        using var lockCts = CancellationTokenSource.
          CreateLinkedTokenSource(ct, workCts.Token);

        await using var exclusiveLockHandler = await services.
          TryAcquireLockAsync(jobName, lockInterval, lockCts.Token);

        if (exclusiveLockHandler is not null)
          await work(lockCts.Token);
      }
      catch (OperationCanceledException) when (ct.IsCancellationRequested)
      {
        return;
      }
      catch (OperationCanceledException) { continue; }
      catch (Exception exception)
      {
        services.InstrumentJobException(jobName, exception);
      }
    }
  }
}