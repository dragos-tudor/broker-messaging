namespace Operations.Inbound.Envelope;

internal sealed class EnvelopeData :
  ICapturingData<string, byte[], object, string>,
  IVerifyingData<string, byte[], object, string>,
  IMappingData<string, byte[], object, string, string>,
  IConvertingData<string, byte[], object, string, string>,
  IConfirmingData<string, byte[], object, string>
{
  public IEnvelope<string, byte[], object, string>? Envelope { get; set; }
  public IInboxMessage<string, string>? InboxMessage { get; set; }
  public IDeadLetterEnvelope<string, byte[], object, string>? DeadLetterEnvelope { get; set; }
}
