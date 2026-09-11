
namespace Operations.Outbound.Envelope;

public record ProduceResult
{
  public Guid MessageId { get; init; }
  public bool IsAcknowledged { get; set; }
  public Exception? Exception { get; set; }
}