
namespace Operations.Inbound.DeadLetter;

public interface IInsertingData<TKey, TPayload>:
  IDeadLetterMessageProp<TKey, TPayload>;