
namespace Operations.Inbound.Inbox;

public interface IUpsertingData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>,
  IPipelineErrorProp;