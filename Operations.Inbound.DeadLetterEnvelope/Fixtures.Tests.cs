
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeTests
{
  static object?[] CreateDeadLetterEnvelopeData<TKey, TValue, TMetadata, TConfirmation>(
    IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope)
  {
    object?[] data = new object?[6];
    if (envelope is not null)
      SetDeadLetterEnvelope(data, envelope);
    return data;
  }

  static object?[] CreateDispatchingData(ProduceResult? result)
  {
    object?[] data = new object?[6];
    if (result is not null)
      DeadLetterEnvelopeFuncs.SetProduceResult(data, result);
    return data;
  }

  static object?[] CreateProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope,
    IDeadLetterMessage<TKey, TPayload>? message)
  {
    object?[] data = new object?[6];
    if (envelope is not null)
      SetDeadLetterEnvelope(data, envelope);
    if (message is not null)
      SetDeadLetterMessage(data, message);
    return data;
  }
}
