
namespace Operations.Inbound.DeadLetter;

public interface IClosingData<TKey, TPayload>:
  IDeadLetterMessageProp<TKey, TPayload>;