using Operations.Inbound.Envelope;

namespace Pipelines.Inbound;

public interface IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>:
  Operations.Inbound.DeadLetterEnvelope.IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>,
  IConvertingServices<TKey, TValue, TMetadata, TConfirmation>,
  IConfirmingServices<TKey, TValue, TMetadata, TConfirmation>;