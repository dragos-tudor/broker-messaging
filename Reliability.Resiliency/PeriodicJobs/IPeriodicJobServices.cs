
namespace Reliability.Resiliency;

public interface IPeriodicJobServices :
  IDistributedLockService,
  ILoggerService;

public interface IDistributedLockService
{
  Task<IAsyncDisposable?> TryAcquireLockAsync(string key, TimeSpan lockDuration, CancellationToken cancellationToken);
}

public interface ILoggerService
{
  ILogger GetLogger();
}