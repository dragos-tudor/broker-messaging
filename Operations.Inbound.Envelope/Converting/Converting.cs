
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (object?[], string, Exception?)
    ConvertEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ConvertingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      object?[] data)
    {
      var envelope = RequireEnvelope(GetEnvelope<TKey, TValue, TMetadata, TConfirmation>(data));
      var message = GetInboxMessage<TKey, TPayload>(data);
      var failureReason = RequireFailureReason(envelope, message);

      var currentDate = capabilities.GetUtcDateTime();
      var deadLetterEnvelope =
        capabilities.FromEnvelope(envelope, failureReason, currentDate);

      SetDeadLetterEnvelope(data, deadLetterEnvelope);
      return (data, ConvertingStates.Success, null);
    }

  static (object?[], string, Exception?)
    ConvertEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, ConvertingStates.Error, exception);

  internal static (object?[], string, Exception?)
    ConvertEnvelope<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ConvertingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      ConvertEnvelopeSuccess,
      ConvertEnvelopeError);
}
