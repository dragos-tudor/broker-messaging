
namespace Operations.Inbound.DeadLetter;

public interface ISchedulingServices<TKey, TPayload>:
  IDeadLetterRetryOptionsReaderService,
  IDeadLetterMessageUpdateService<TKey, TPayload>,
  IUtcDateService;