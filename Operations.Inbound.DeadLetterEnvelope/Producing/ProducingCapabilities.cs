namespace Operations.Inbound.DeadLetterEnvelope;

sealed record ProducingCapabilities<TKey, TValue, TMetadata, TConfirmation>(
  ProduceDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>
    ProduceDeadLetterEnvelope,
  DispatchProduceResult DispatchProduceResult
);
