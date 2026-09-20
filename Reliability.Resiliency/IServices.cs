
namespace Reliability.Resiliency;

public interface IDistributedLockService
{
  Task<IAsyncDisposable?> TryAcquireLockAsync(string key, TimeSpan lockDuration, CancellationToken cancellationToken);
}

public interface IResiliencyInstrumentionServices
{
  void InstrumentJobException(string jobName, Exception exception);
}


