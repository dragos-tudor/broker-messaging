
namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  static bool SetProduceResultIsAcknowledged(
    ProduceResult result,
    bool isAcknowledged) =>
      result.IsAcknowledged = isAcknowledged;

  static Exception? SetProduceResultException(
    ProduceResult result,
    Exception? exception) =>
      result.Exception = exception;
}