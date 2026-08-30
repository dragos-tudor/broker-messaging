
namespace Operations.Outbound.Outbox;

public interface ISchedulingData<TKey, TPayload>:
  IOutboxMessageProp<TKey, TPayload>,
  IPipelineErrorProp;