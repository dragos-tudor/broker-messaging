
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public interface IPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IPersistingData<TKey, TPayload>,
  IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IDispatchingData<TKey, TValue, TMetadata, TConfirmation, TPayload>;

public class PipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload>
{
  public object? DomainModel { get; set; }
  public IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope { get; set; }
  public IOutboxMessage<TKey, TPayload>? OutboxMessage { get; set; }
  public ProduceResult? ProduceResult { get; set; }
}
