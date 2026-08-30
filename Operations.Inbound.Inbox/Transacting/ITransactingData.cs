
namespace Operations.Inbound.Inbox;

public interface ITransactingData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>,
  IModelProp,
  IPipelineErrorProp;