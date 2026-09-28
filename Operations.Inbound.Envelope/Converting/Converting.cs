
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (object?[], string, Exception?)
    ConvertEnvelopeSuccess(
      ConvertingCapabilities capabilities,
      object?[] data)
    {
      var envelope = RequireEnvelope(GetEnvelope(data));
      var message = GetInboxMessage(data);
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
    ConvertEnvelope(
      ConvertingCapabilities capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      ConvertEnvelopeSuccess,
      ConvertEnvelopeError);
}
