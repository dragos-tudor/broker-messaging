
namespace Operations.Inbound.Inbox;

public interface IInsertingServices<TKey, TPayload> :
  IInboxMessageInsertService<TKey, TPayload>;

public interface IInboxMessageInsertService<TKey, TPayload>
{
  Task<bool> InsertInboxMessageAsync(InboxMessage<TKey, TPayload> message, CancellationToken ct = default);
}
