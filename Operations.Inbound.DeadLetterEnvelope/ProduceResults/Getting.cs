namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  const int ProduceResultIndex = 4;

  internal static ProduceResult? GetProduceResult(object?[] data) =>
    (ProduceResult?)data[ProduceResultIndex];
}
