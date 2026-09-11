
namespace Operations.Outbound.Envelope;

public interface IProduceResultProp {
  ProduceResult? ProduceResult { get; set; }
}

public interface IEnvelopeProp<TKey, TData, TMetadata, TConfirmation> {
  IEnvelope<TKey, TData, TMetadata, TConfirmation>? Envelope { get; set; }
}

public interface IOutboxMessageProp<TKey, TPayload> {
  IOutboxMessage<TKey, TPayload>? OutboxMessage { get; set; }
}