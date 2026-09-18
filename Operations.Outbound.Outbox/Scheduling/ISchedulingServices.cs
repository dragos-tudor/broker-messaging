
namespace Operations.Outbound.Outbox;

public interface ISchedulingServices<TKey, TPayload>:
  IOutboxRetryOptionsReaderService,
  IOutboxMessageUpdateService<TKey, TPayload>,
  IUtcDateService;
