
namespace Operations.Outbound.Outbox;

public interface ISchedulingServices<TKey, TPayload>:
  IOutboxMessageOptionsReaderService,
  IOutboxMessageUpdateService<TKey, TPayload>,
  IUtcDateService;

public interface IUtcDateService { DateTime GetUtcDateTime(); }