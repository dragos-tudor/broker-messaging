namespace Operations.Outbound.Envelope;

public sealed record ProducingCapabilities(
  ProduceEnvelope ProduceEnvelope,
  DispatchProduceResult DispatchProduceResult
);
