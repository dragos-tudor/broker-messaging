
namespace Reliability.Resiliency;

public delegate FastRetryOptions GetFastRetryOptions();

public delegate  Task<bool> IsFastRetryDelayedAsync(
  int retryCount,
  FastRetryOptions options,
  CancellationToken ct = default);

public delegate Task<IAsyncDisposable?> TryAcquireLockAsync(string key, TimeSpan lockDuration, CancellationToken cancellationToken);

public delegate void InstrumentJobException(string jobName, Exception exception);
