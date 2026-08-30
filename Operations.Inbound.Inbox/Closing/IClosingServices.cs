
namespace Operations.Inbound.Inbox;

 public interface IClosingServices<TKey, TPayload> :
  IInboxMessageUpdateService<TKey, TPayload>;

