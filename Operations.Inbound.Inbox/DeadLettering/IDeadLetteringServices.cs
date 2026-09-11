
namespace Operations.Inbound.Inbox;

 public interface IDeadLetteringServices<TKey, TPayload> :
  IInboxMessageUpdateService<TKey, TPayload>;

