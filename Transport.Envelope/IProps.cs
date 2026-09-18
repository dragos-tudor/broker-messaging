
namespace Transport.Envelope;

public interface IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>
{
  IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope { get; set; }
}