
namespace Operations.Outbound.Envelope;

partial class EnvelopeTests
{
  static object?[] CreateEnvelopeData(
    IEnvelope? envelope)
  {
    object?[] data = new object?[4];
    if (envelope is not null)
      SetEnvelope(data, envelope);
    return data;
  }

  static object?[] CreateProducingData(
    IEnvelope? envelope,
    IOutboxMessage? message)
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
