
namespace Operations.Inbound.DeadLetter;

public interface IClosingServices<TKey, TPayload>:
  IDeadLetterMessageUpdateService<TKey, TPayload>;