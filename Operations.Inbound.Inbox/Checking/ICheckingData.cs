
namespace Operations.Inbound.Inbox;

internal interface ICheckingData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>,
  IPipelineErrorProp;