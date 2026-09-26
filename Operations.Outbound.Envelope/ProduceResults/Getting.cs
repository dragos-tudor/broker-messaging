namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  const int ProduceResultIndex = 2;

  internal static ProduceResult? GetProduceResult(object?[] objects) =>
    (ProduceResult?)objects[ProduceResultIndex];
}
