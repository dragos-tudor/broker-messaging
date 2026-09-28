namespace Operations.Inbound.DeadLetterEnvelope;

public sealed record ProducingCapabilities(
  ProduceDeadLetterEnvelope
    ProduceDeadLetterEnvelope,
  DispatchProduceResult DispatchProduceResult
);
