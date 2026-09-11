using Operations.Inbound.Envelope;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public interface IRedirectingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IRedirectingData<TKey, TValue, TMetadata, TConfirmation>,
  IConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IConfirmingData<TKey, TValue, TMetadata, TConfirmation>;