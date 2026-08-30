
namespace Operations.Inbound.Inbox;

public interface IClosingData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>,
  IPipelineErrorProp;