namespace Operations.Inbound.Envelope;

public interface IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation> {
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? DeadLetterEnvelope { get; set; }
}

public interface IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation> {
  IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope { get; set; }
}

public interface IInboxMessageProp<TKey, TPayload> { InboxMessage<TKey, TPayload>? InboxMessage { get; set; } }

public interface IPipelineErrorProp { string? PipelineError { get; set; } }
