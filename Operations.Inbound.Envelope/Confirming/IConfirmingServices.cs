
namespace Operations.Inbound.Envelope;

public interface IConfirmingServices<TKey, TValue, TMetadata, TConfirmation> :
  IEnvelopeConfirmationService<TKey, TValue, TMetadata, TConfirmation>;