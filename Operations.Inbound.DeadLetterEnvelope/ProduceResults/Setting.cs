
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
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