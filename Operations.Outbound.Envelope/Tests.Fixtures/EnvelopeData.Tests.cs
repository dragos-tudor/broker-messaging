namespace Operations.Outbound.Envelope;

internal sealed class EnvelopeData :
  IDispatchingData,
  IProducingData<string, byte[], object, string, string>,
  IPublishingData<string, byte[], object, string, string>
{
  public IEnvelope<string, byte[], object, string>? Envelope { get; set; }
  public IOutboxMessage<string, string>? OutboxMessage { get; set; }
  public ProduceResult? ProduceResult { get; set; }
}
