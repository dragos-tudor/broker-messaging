using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public sealed record CapturingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  CapturingCapabilities<TKey, TValue, TMetadata, TConfirmation> Capturing,
  VerifyingCapabilities<TKey, TValue, TMetadata, TConfirmation> Verifying,
  MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> Mapping,
  ValidatingCapabilities<TKey, TPayload> Validating,
  InsertingCapabilities<TKey, TPayload> Inserting,
  ConfirmingCapabilities<TKey, TValue, TMetadata, TConfirmation> Confirming
);