
namespace Operations.Inbound.DeadLetter;

public interface IInsertingServices<TKey, TPayload> :
  IDeadLetterMessageInsertService<TKey, TPayload>;