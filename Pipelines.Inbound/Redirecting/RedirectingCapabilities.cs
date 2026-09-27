using Operations.Inbound.Envelope;

namespace Pipelines.Inbound;

public sealed record RedirectingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>
(
  Operations.Inbound.DeadLetterEnvelope.RedirectingCapabilities<TKey, TValue, TMetadata, TConfirmation> Redirecting,
  ConvertingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> Converting,
  ConfirmingCapabilities<TKey, TValue, TMetadata, TConfirmation> Confirming
);