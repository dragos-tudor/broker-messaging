namespace Operations.Inbound.DeadLetterEnvelope;

internal sealed class DeadLetterEnvelopeData :
  IDispatchingData,
  IProducingData<string, byte[], object, string, string>,
  IPublishingData<string, byte[], object, string>,
  IRedirectingData<string, byte[], object, string>
{
  public IDeadLetterEnvelope<string, byte[], object, string>? DeadLetterEnvelope { get; set; }
  public IDeadLetterMessage<string, string>? DeadLetterMessage { get; set; }
  public ProduceResult? ProduceResult { get; set; }
}
