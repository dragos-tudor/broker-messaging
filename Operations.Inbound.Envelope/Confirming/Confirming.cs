
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static async Task<(TData, ConfirmingStates, Exception?)> ConfirmEnvelopeSuccess<TService, TData, TKey, TValue, TMetadata, TConfirmation>(
    TService services,
    TData data,
    CancellationToken ct = default)
  where TService : IConfirmingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IConfirmingData<TKey, TValue, TMetadata, TConfirmation>
  {
    var envelope = RequireEnvelope(data.Envelope);

    await services.ConfirmEnvelope(envelope, ct);
    return (data, ConfirmingStates.Success, null);
  }

  static (TData, ConfirmingStates, Exception?) ConfirmEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation>(
    TData data,
    Exception? exception)
  where TData : IConfirmingData<TKey, TValue, TMetadata, TConfirmation> =>
    (data, ConfirmingStates.Error, exception);

  internal static Task<(TData, ConfirmingStates, Exception?)> ConfirmEnvelope<TService, TData, TKey, TValue, TMetadata, TConfirmation>(
    TService services,
    TData data,
    CancellationToken ct = default)
  where TService : IConfirmingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IConfirmingData<TKey, TValue, TMetadata, TConfirmation> =>
     TryCatch(
      services,
      data,
      ConfirmEnvelopeSuccess<TService, TData, TKey, TValue, TMetadata, TConfirmation>,
      ConfirmEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation>,
      ct);

  internal static async Task<(TData, ConfirmingFinalStates, Exception?)> ConfirmFinalEnvelope<TService, TData, TKey, TValue, TMetadata, TConfirmation>(
    TService services,
    TData data,
    CancellationToken ct = default)
  where TService : IConfirmingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IConfirmingData<TKey, TValue, TMetadata, TConfirmation> =>
    await ConfirmEnvelope<TService, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct) switch
    {
      (var confirmedData, ConfirmingStates.Success, null) => new(confirmedData, ConfirmingFinalStates.Success, null),
      (var confirmedData, ConfirmingStates.Error, var exception) => new(confirmedData, ConfirmingFinalStates.Error, exception),
      (var confirmedData, var _, var exception) => new(confirmedData, default, exception)
    };
}
