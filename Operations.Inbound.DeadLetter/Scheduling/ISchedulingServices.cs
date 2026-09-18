
namespace Operations.Inbound.DeadLetter;

public interface ISchedulingServices<TKey, TPayload>:
  IDeadLetterRetryOptionsService,
  IDeadLetterMessageUpdateService<TKey, TPayload>,
  IUtcDateService;