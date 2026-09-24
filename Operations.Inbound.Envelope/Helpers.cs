
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static string RequireFailureReason<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    ConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data) =>
      data.InboxMessage?.FailureReason ??
      data.Envelope?.FailureReason ??
      throw new InvalidOperationException($"Missing failure reason for dead letter envelope.");

  internal static IEnvelope<TKey, TValue, TMetadata, TConfirmation> RequireEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope) =>
    envelope ?? throw new InvalidOperationException("Envelope is required.");
}
