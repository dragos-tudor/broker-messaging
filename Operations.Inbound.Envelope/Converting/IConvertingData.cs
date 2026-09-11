
namespace Operations.Inbound.Envelope;

public interface IConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IInboxMessageProp<TKey, TPayload>;