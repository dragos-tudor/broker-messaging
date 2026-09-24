namespace Operations.Inbound.DeadLetterEnvelope;

readonly record struct DispatchingData(
  ProduceResult? ProduceResult
);
