
namespace Operations.Inbound.Inbox;

public interface IHandlingData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>,
  IDomainModelProp;