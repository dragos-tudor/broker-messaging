
namespace Operations.Outbound.Envelope;

public interface IEnvelopeProp<TKey, TData, TMetadata, TConfirmation> {
  IEnvelope<TKey, TData, TMetadata, TConfirmation>? Envelope { get; set; }
}

public interface IOutboxMessageProp<TKey, TPayload> { OutboxMessage<TKey, TPayload>? OutboxMessage { get; set; } }

public interface IPipelineErrorProp { string? PipelineError { get; set; } }