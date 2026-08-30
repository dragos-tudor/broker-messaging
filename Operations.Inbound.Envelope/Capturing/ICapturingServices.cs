
namespace Operations.Inbound.Envelope;

public interface ICapturingServices<TKey, TValue, TMetadata, TConfirmation> :
  IEnvelopeReaderService<TKey, TValue, TMetadata, TConfirmation>;
