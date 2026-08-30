
namespace Operations.Outbound.Outbox;

public interface IAbandoningServices<TKey, TPayload>:
  IOutboxMessageUpdateService<TKey, TPayload>;