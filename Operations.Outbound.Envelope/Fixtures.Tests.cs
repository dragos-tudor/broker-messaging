
namespace Operations.Outbound.Envelope;

partial class EnvelopeTests
{
  static object?[] CreateEnvelopeData<TKey, TValue, TMetadata, TConfirmation>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope)
  {
    object?[] data = new object?[4];
    if (envelope is not null)
      SetEnvelope(data, envelope);
    return data;
  }

  static object?[] CreateProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope,
    IOutboxMessage<TKey, TPayload>? message)
  {
    object?[] data = new object?[4];
    if (envelope is not null)
      SetEnvelope(data, envelope);
    if (message is not null)
      SetOutboxMessage(data, message);
    return data;
  }

  static object?[] CreateDispatchingData(ProduceResult? result)
  {
    object?[] data = new object?[4];
    if (result is not null)
      EnvelopeFuncs.SetProduceResult(data, result);
    return data;
  }
}
