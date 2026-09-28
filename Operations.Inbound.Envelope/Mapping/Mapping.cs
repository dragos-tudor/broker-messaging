namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (object?[], string, Exception?)
    MapEnvelopeSuccess(
      MappingCapabilities capabilities,
      object?[] data)
    {
      var envelope = RequireEnvelope(GetEnvelope(data));
      var currentDate = capabilities.GetUtcDateTime();

      var message = capabilities.FromEnvelope(envelope, currentDate);

      SetInboxMessage(data, message);
      return (data, MappingStates.Success, null);
    }

  static (object?[], string, Exception?)
    MapEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, MappingStates.Error, exception);

  internal static (object?[], string, Exception?)
    MapEnvelope(
      MappingCapabilities capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      MapEnvelopeSuccess,
      MapEnvelopeError);
}
