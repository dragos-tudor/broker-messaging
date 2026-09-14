
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static async ValueTask<(TData, CapturingStates, Exception?)> CaptureEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ICapturingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : ICapturingData<TKey, TValue, TMetadata, TConfirmation>
  {
    var envelope = await services.ReadEnvelope(ct);

    return SetEnvelope(data, envelope) is not null ?
      new(data, CapturingSuccess, null) :
      new(data, CapturingNotCaptured, null);
  }

  static (TData, CapturingStates, Exception?) CaptureEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation>(
    TData data,
    Exception exception)
  where TData : ICapturingData<TKey, TValue, TMetadata, TConfirmation>
    => (data, CapturingError, exception);

  internal static ValueTask<(TData, CapturingStates, Exception?)> CaptureEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ICapturingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : ICapturingData<TKey, TValue, TMetadata, TConfirmation> =>
    TryCatch(
      services,
      data,
      CaptureEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      CaptureEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation>,
      ct
    );
}
