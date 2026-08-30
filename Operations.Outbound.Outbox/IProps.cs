
namespace Operations.Outbound.Outbox;

public interface IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation> {
  IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope { get; set; }
}

public interface IModelProp { object Model { get; init; } }

public interface IOutboxMessageProp<TKey, TPayload> {
  OutboxMessage<TKey, TPayload>? OutboxMessage { get; set; }
}