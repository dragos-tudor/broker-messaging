
namespace Transport.DeadLetterEnvelope;

public interface IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>
{
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? DeadLetterEnvelope { get; set; }
}