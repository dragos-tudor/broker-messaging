
namespace Operations.Inbound.Inbox;

public interface ISchedulingData<TKey, TPayload>:
  IPipelineErrorProp,
  IInboxMessageProp<TKey, TPayload>;