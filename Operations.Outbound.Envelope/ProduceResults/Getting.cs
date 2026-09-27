namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  const int ProduceResultIndex = 3;

  internal static ProduceResult? GetProduceResult(object?[] objects) =>
    (ProduceResult?)objects[ProduceResultIndex];
}
