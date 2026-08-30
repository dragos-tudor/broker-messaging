
namespace Operations.Inbound.Inbox;

public interface IInsertingData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>,
  IPipelineErrorProp;