namespace Operations.Inbound.DeadLetter;

internal sealed class DeadLetterData :
  IAbandoningData<string, string>,
  IClosingData<string, string>,
  IInsertingData<string, string>,
  IMappingData<string, byte[], object, string, string>,
  ISchedulingData<string, string>
{
  public IDeadLetterMessage<string, string>? DeadLetterMessage { get; set; }
  public IDeadLetterEnvelope<string, byte[], object, string>? DeadLetterEnvelope { get; set; }
}
