namespace Operations.Outbound.Envelope;

readonly record struct DispatchingData(
  ProduceResult? ProduceResult
);
