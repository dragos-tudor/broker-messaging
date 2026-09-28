
namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  static IEnvelope RequireEnvelope(
    IEnvelope? message) =>
      message ?? throw new InvalidOperationException("Envelope is required.");

  static IOutboxMessage RequireOutboxMessage(
    IOutboxMessage? message) =>
      message ?? throw new InvalidOperationException("Outbox message is required.");

  static ProduceResult RequireProduceResult(
    ProduceResult? result) =>
      result ?? throw new InvalidOperationException("Produce result is required.");
}