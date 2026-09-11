
namespace Operations.Inbound.DeadLetter;

public interface IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>
{
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? DeadLetterEnvelope { get; set; }
}

public interface IDeadLetterMessageProp<TKey, TPayload>
{
  IDeadLetterMessage<TKey, TPayload>? DeadLetterMessage { get; set; }
}