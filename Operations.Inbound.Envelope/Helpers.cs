
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static string RequireFailureReason<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope,
    IInboxMessage<TKey, TPayload>? message) =>
      message?.FailureReason ??
      envelope?.FailureReason ??
      throw new InvalidOperationException($"Missing failure reason for dead letter envelope.");

  internal static IEnvelope<TKey, TValue, TMetadata, TConfirmation> RequireEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope) =>
    envelope ?? throw new InvalidOperationException("Envelope is required.");
}
