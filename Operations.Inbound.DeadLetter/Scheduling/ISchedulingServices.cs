
namespace Operations.Inbound.DeadLetter;

public interface ISchedulingServices<TKey, TPayload>:
  IDeadLetterMessageOptionsReaderService,
  IDeadLetterMessageUpdateService<TKey, TPayload>,
  IUtcDateService;

public interface IUtcDateService { DateTime GetUtcDateTime(); }