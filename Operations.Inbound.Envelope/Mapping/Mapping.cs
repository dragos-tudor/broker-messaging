
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (TData, string, Exception?) MapEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data)
  where TServices : IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    var envelope = RequireEnvelope(data.Envelope);

    var message = services.FromEnvelope(envelope, services.GetUtcDateTime());
    SetInboxMessage(data, message);

    return (data, MappingSuccess, null);
  }

  static (TData, string, Exception?) MapEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    Exception exception)
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    (data, MappingError, exception);

  internal static ValueTask<(TData, string, Exception?)> MapEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  =>
    TryCatch(
      services,
      data,
      MapEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      MapEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>);
}
