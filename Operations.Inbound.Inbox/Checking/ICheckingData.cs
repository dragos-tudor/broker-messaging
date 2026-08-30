
namespace Operations.Inbound.Inbox;

public interface ICheckingData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>,
  IPipelineErrorProp;