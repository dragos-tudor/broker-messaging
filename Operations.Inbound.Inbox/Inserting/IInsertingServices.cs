
namespace Operations.Inbound.Inbox;

public interface IInsertingServices<TKey, TPayload> :
  IInboxMessageInsertService<TKey, TPayload>;

