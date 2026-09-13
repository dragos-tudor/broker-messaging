
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public interface IPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  ICapturingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IRedirectingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IHandlingData<TKey, TPayload>,
  IDeadLetteringData<TKey, TPayload>,
  IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IDispatchingData<TKey, TValue, TMetadata, TConfirmation, TPayload>;

public class PipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload>
{
  public IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? DeadLetterEnvelope { get; set; }
  public IDeadLetterMessage<TKey, TPayload>? DeadLetterMessage { get; set; }
  public object? DomainModel { get; set; }
  public IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope { get; set; }
  public IInboxMessage<TKey, TPayload>? InboxMessage { get; set; }
  public ProduceResult? ProduceResult { get; set; }
}
