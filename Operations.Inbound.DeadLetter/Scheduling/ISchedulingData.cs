
namespace Operations.Inbound.DeadLetter;

public interface ISchedulingData<TKey, TPayload>:
  IDeadLetterMessageProp<TKey, TPayload>,
  IPipelineErrorProp;