
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static IDeadLetterEnvelope RequireDeadLetterEnvelope(
    IDeadLetterEnvelope? envelope) =>
    envelope ?? throw new InvalidOperationException("Dead letter envelope is required.");

  static IDeadLetterMessage RequireDeadLetterMessage(
    IDeadLetterMessage? message) =>
    message ?? throw new InvalidOperationException("Dead letter message is required.");

  static ProduceResult RequireProduceResult(
    ProduceResult? result) =>
      result ?? throw new InvalidOperationException("Produce result is required.");
}