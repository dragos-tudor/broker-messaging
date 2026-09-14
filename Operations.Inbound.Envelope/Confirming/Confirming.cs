
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static async ValueTask<(TData, ConfirmingStates, Exception?)> ConfirmEnvelopeSuccess<TService, TData, TKey, TValue, TMetadata, TConfirmation>(
    TService services,
    TData data,
    CancellationToken ct = default)
  where TService : IConfirmingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IConfirmingData<TKey, TValue, TMetadata, TConfirmation>
  {
    var envelope = RequireEnvelope(data.Envelope);

    await services.ConfirmEnvelope(envelope, ct);
    return (data, ConfirmingSuccess, null);
  }

  static (TData, ConfirmingStates, Exception?) ConfirmEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation>(
    TData data,
    Exception? exception)
  where TData : IConfirmingData<TKey, TValue, TMetadata, TConfirmation> =>
    (data, ConfirmingError, exception);

  internal static ValueTask<(TData, ConfirmingStates, Exception?)> ConfirmEnvelope<TService, TData, TKey, TValue, TMetadata, TConfirmation>(
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

  internal static async ValueTask<(TData, ConfirmingFinalStates, Exception?)> ConfirmFinalEnvelope<TService, TData, TKey, TValue, TMetadata, TConfirmation>(
    TService services,
    TData data,
    CancellationToken ct = default)
  where TService : IConfirmingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IConfirmingData<TKey, TValue, TMetadata, TConfirmation> =>
    await ConfirmEnvelope<TService, TData, TKey, TValue, TMetadata, TConfirmation>(services, data, ct) switch
    {
      (var confirmedData, ConfirmingSuccess, null) => new(confirmedData, ConfirmingFinalSuccess, null),
      (var confirmedData, ConfirmingError, var exception) => new(confirmedData, ConfirmingFinalError, exception),
      (var confirmedData, var _, var exception) => new(confirmedData, default, exception)
    };
}
