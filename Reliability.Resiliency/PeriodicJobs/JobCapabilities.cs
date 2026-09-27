
namespace Reliability.Resiliency;

public sealed record JobCapabilities
(
  TryAcquireLockAsync TryAcquireLockAsync,
  InstrumentJobException InstrumentJobException
);
