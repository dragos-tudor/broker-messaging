
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static string RequireFailureReason(
    IEnvelope? envelope,
    IInboxMessage? message) =>
      message?.FailureReason ??
      envelope?.FailureReason ??
      throw new InvalidOperationException($"Missing failure reason for dead letter envelope.");

  internal static IEnvelope RequireEnvelope(
    IEnvelope? envelope) =>
    envelope ?? throw new InvalidOperationException("Envelope is required.");
}
