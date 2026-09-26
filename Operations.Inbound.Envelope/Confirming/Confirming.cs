namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static async Task<(ConfirmingData<TKey, TValue, TMetadata, TConfirmation>, string, Exception?)>
    ConfirmEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation>(
      ConfirmingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      ConfirmingData<TKey, TValue, TMetadata, TConfirmation> data,
      CancellationToken ct = default)
    {
      var envelope = RequireEnvelope(data.Envelope);

      await capabilities.ConfirmEnvelope(envelope, ct);

      return (data, ConfirmingStates.Success, null);
    }

  static (ConfirmingData<TKey, TValue, TMetadata, TConfirmation>, string, Exception?)
    ConfirmEnvelopeError<TKey, TValue, TMetadata, TConfirmation>(
      ConfirmingData<TKey, TValue, TMetadata, TConfirmation> data,
      Exception exception) =>
    (data, ConfirmingStates.Error, exception);

  internal static Task<(ConfirmingData<TKey, TValue, TMetadata, TConfirmation>, string, Exception?)>
    ConfirmEnvelope<TKey, TValue, TMetadata, TConfirmation>(
      ConfirmingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      ConfirmingData<TKey, TValue, TMetadata, TConfirmation> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      ConfirmEnvelopeSuccess,
      ConfirmEnvelopeError,
      ct);

  internal static async Task<(ConfirmingData<TKey, TValue, TMetadata, TConfirmation>, string, Exception?)>
    ConfirmFinalEnvelope<TKey, TValue, TMetadata, TConfirmation>(
      ConfirmingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      ConfirmingData<TKey, TValue, TMetadata, TConfirmation> data,
      CancellationToken ct = default) =>
    await ConfirmEnvelope(capabilities, data, ct) switch
    {
      (var confirmedData, ConfirmingStates.Success, null) =>
        (confirmedData, ConfirmingFinalStates.Success, null),
      (var confirmedData, ConfirmingStates.Error, var exception) =>
        (confirmedData, ConfirmingFinalStates.Error, exception),
      (var confirmedData, _, var exception) =>
        (confirmedData, ConfirmingFinalStates.Success, exception)
    };
}
