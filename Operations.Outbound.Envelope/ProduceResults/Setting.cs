
namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  internal static object SetProduceResult(object?[] data, ProduceResult result) =>
    data[ProduceResultIndex] = result;

  static bool SetProduceResultIsAcknowledged(
    ProduceResult result,
    bool isAcknowledged) =>
      result.IsAcknowledged = isAcknowledged;

  static Exception? SetProduceResultException(
    ProduceResult result,
    Exception? exception) =>
      result.Exception = exception;
}