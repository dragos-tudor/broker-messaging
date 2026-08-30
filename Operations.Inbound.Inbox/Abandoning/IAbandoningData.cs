
namespace Operations.Inbound.Inbox;

public interface IAbandoningData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>,
  IPipelineErrorProp;