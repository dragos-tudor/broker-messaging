
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

    while (true)
    {
      try
      {
        if(!await timer.WaitForNextTickAsync(ct))
          return;

        using var cts = new CancellationTokenSource(
          lockInterval - SafeClosingInterval);

        using var lockCts =
          CancellationTokenSource.CreateLinkedTokenSource(
              ct,
              cts.Token);

        await using var handle =
          await services.TryAcquireLockAsync(
              jobName,
              lockInterval,
              lockCts.Token);

        if (handle is not null)
          await work(lockCts.Token);
      }
      catch (OperationCanceledException) when (ct.IsCancellationRequested)
      {
        return;
      }
      catch (OperationCanceledException) { continue; }
      catch (Exception exception)
      {
        LogPeriodicJobError(
            services.GetLogger(),
            jobName,
            exception);
      }
    }
  }
}