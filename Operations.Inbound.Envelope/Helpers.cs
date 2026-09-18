
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static string RequireFailureReason<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    IConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data) =>
      data.InboxMessage?.FailureReason ??
      data.Envelope?.FailureReason ??
      throw new InvalidOperationException($"Missing failure reason for dead letter envelope.");

  internal static IEnvelope<TKey, TValue, TMetadata, TConfirmation> RequireEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope) =>
    envelope ?? throw new InvalidOperationException("Envelope is required.");
}

partial class EnvelopeFuncs
{
  static IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> SetDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation> data,
    IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope) =>
      data.DeadLetterEnvelope = envelope;

  static IEnvelope<TKey, TValue, TMetadata, TConfirmation> SetEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation> data,
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope) =>
      data.Envelope = envelope;

  static IInboxMessage<TKey, TPayload> SetInboxMessage<TKey, TPayload>(
    IInboxMessageProp<TKey, TPayload> data,
    IInboxMessage<TKey, TPayload> message) =>
      data.InboxMessage = message;
}