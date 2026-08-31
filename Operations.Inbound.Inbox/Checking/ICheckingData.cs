
namespace Operations.Inbound.Inbox;

public interface ICheckingRetryData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>,
  IRetryMessageProp,
  IPipelineErrorProp;