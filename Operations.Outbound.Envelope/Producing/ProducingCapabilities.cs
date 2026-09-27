namespace Operations.Outbound.Envelope;

public sealed record ProducingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  ProduceEnvelope<TKey, TValue, TMetadata, TConfirmation>
    ProduceEnvelope,
  DispatchProduceResult DispatchProduceResult
);
