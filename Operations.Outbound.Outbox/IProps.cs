
namespace Operations.Outbound.Outbox;

public interface IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation> {
  IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope { get; set; }
}

public interface IDomainModelProp {
  object? DomainModel { get; set; }
}

public interface IOutboxMessageProp<TKey, TPayload> {
  IOutboxMessage<TKey, TPayload>? OutboxMessage { get; set; }
}