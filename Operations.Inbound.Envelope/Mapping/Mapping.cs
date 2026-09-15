
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (TData, MappingStates, Exception?) MapEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data)
  where TServices : IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    var envelope = RequireEnvelope(data.Envelope);

    var message = services.FromEnvelope(envelope, services.GetUtcDateTime());
    SetInboxMessage(data, message);

    return (data, MappingStates.Success, null);
  }

  static (TData, MappingStates, Exception?) MapEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    Exception exception)
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    (data, MappingStates.Error, exception);

  internal static ValueTask<(TData, MappingStates, Exception?)> MapEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
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
