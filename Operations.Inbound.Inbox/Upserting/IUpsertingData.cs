
namespace Operations.Inbound.Inbox;

internal interface IUpsertingData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>,
  IPipelineErrorProp;