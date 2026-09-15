
namespace Operations.Inbound.Envelope;

public interface IVerifyingData<TKey, TValue, TMetadata, TConfirmation> :
  IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>;