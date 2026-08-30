
namespace Operations.Outbound.Outbox;

public interface IClosingServices<TKey, TPayload>:
  IOutboxMessageUpdateService<TKey, TPayload>;