
namespace Operations.Outbound.Outbox;

public interface IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IOutboxMessageProp<TKey, TPayload>,
  IPipelineErrorProp;

public interface IPipelineErrorProp { string? PipelineError { get; set; } }