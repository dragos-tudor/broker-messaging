namespace Operations.Outbound.Envelope;

sealed record ProducingCapabilities<TKey, TValue, TMetadata, TConfirmation>(
  ProduceEnvelope<TKey, TValue, TMetadata, TConfirmation>
    ProduceEnvelope,
  DispatchProduceResult DispatchProduceResult
);
