namespace Operations.Outbound.Outbox;

internal sealed class OutboxData :
  IAbandoningData<string, string>,
  IClosingData<string, string>,
  IMappingData<string, byte[], object, string, string>,
  ISchedulingData<string, string>,
  ITransactingData<string, string>,
  IValidatingData<string, string>
{
  public IOutboxMessage<string, string>? OutboxMessage { get; set; }
  public IEnvelope<string, byte[], object, string>? Envelope { get; set; }
  public object? DomainModel { get; set; }
}
