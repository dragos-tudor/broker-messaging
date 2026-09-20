
namespace Reliability.Resiliency;

public interface IPeriodicJobServices :
  IDistributedLockService,
  IResiliencyInstrumentionServices;
