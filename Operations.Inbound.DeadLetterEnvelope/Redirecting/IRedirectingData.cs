
namespace Operations.Inbound.DeadLetterEnvelope;

public interface IRedirectingData<TKey, TValue, TMetadata, TConfirmation>:
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>;