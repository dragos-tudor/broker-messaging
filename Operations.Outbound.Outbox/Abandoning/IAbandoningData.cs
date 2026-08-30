
namespace Operations.Outbound.Outbox;

public interface IAbandoningData<TKey, TPayload>:
  IOutboxMessageProp<TKey, TPayload>,
  IPipelineErrorProp;