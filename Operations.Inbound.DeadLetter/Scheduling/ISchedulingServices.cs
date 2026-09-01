
namespace Operations.Inbound.DeadLetter;

public interface ISchedulingServices<TKey, TPayload>:
  IDeadLetterMessageOptionsReaderService,
  IDeadLetterMessageUpdateService<TKey, TPayload>,
  IUtcDateService;