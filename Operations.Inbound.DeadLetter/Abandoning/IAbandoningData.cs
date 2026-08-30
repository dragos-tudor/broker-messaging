
namespace Operations.Inbound.DeadLetter;

public interface IAbandoningData<TKey, TPayload>:
  IDeadLetterMessageProp<TKey, TPayload>,
  IPipelineErrorProp;