
namespace Operations.Inbound.Inbox;

 public interface IAbandoningServices<TKey, TPayload> :
  IInboxMessageUpdateService<TKey, TPayload>;

