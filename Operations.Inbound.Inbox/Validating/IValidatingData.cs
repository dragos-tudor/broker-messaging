
namespace Operations.Inbound.Inbox;

public interface IValidatingData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>,
  IPipelineErrorProp;