
namespace Operations.Inbound.DeadLetter;

public interface IAbandoningServices<TKey, TPayload>:
  IDeadLetterMessageUpdateService<TKey, TPayload>;