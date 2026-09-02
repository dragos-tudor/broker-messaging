
namespace Pipelines.Inbound;

public interface IInboundPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IDeadLetterData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IDeadLetterEnvelopeData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IEnvelopeData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IInboxData<TKey, TValue, TMetadata, TConfirmation, TPayload>;

public class InboundPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IInboundPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload>
{
  public IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope { get; set; }
  public IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? DeadLetterEnvelope { get; set; }
  public InboxMessage<TKey, TPayload>? InboxMessage { get; set; }
  public RetryPlan? RetryPlan { get; set; }
  public DeadLetterMessage<TKey, TPayload>? DeadLetterMessage { get; set; }
  public object? Model { get; set; }
  public string? PipelineError { get; set; } = string.Empty;
}
