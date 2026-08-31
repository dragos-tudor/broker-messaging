
namespace Operations.Inbound.Inbox;

public interface IUpsertingRetryData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>,
  IRetryMessageProp,
  IPipelineErrorProp;