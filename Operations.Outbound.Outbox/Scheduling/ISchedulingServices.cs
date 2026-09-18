
namespace Operations.Outbound.Outbox;

public interface ISchedulingServices<TKey, TPayload>:
  IOutboxRetryOptionsService,
  IOutboxMessageUpdateService<TKey, TPayload>,
  IUtcDateService;
