namespace Operations.Inbound.DeadLetterEnvelope;

public sealed record ProducingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  ProduceDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>
    ProduceDeadLetterEnvelope,
  DispatchProduceResult DispatchProduceResult
);
