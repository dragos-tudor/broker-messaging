
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  internal static object SetProduceResult(object?[] objects, ProduceResult result) =>
    objects[ProduceResultIndex] = result;

  static bool SetProduceResultIsAcknowledged(
    ProduceResult result,
    bool isAcknowledged) =>
      result.IsAcknowledged = isAcknowledged;

  static Exception? SetProduceResultException(
    ProduceResult result,
    Exception? exception) =>
      result.Exception = exception;
}