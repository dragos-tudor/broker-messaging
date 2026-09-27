using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public sealed record PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> Mapping,
  ProducingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> Producing,
  PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation> Publishing,
  SchedulingCapabilities<TKey, TPayload> Scheduling,
  AbandoningCapabilities<TKey, TPayload> Abandoning,
  ClosingCapabilities<TKey, TPayload> Closing
);
