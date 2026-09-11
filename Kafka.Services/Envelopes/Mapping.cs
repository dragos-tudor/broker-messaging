
namespace Kafka.Services;

partial class ServicesFuncs
{
  public static InboxMessage<TKey, TPayload> ToInboxMessage<TKey, TValue, TPayload>(
    Envelope<TKey, TValue> envelope,
    TPayload payload)
  =>
    CreateInboxMessage(
      GetKafkaHeaderMessageId(envelope.Metadata) ?? Guid.NewGuid(),
      TryGetTransportMessageId(envelope.Confirmation),
      envelope.Key,
      payload,
      envelope.CreatedAt,
      GetKafkaHeaderSchemaType(envelope.Metadata) ?? typeof(TPayload).Name,
      GetKafkaHeaderCorrelationId(envelope.Metadata),
      GetKafkaHeaderSchemaVersion(envelope.Metadata),
      TrySerializeTopicPartitionOffset(envelope.Confirmation));

  public static Envelope<TKey, TValue> FromOutboxMessage<TKey, TValue, TPayload>(
    OutboxMessage<TKey, TPayload> message,
    TValue value,
    string queueName)
  =>
    CreateEnvelope(
      CreateKafkaMessage(
        message.MessageKey,
        value,
        SetKafkaMessageHeaders(
          [],
          message.MessageId,
          message.Type,
          message.Version,
          message.CorrelationId
        ),
        message.CreatedAt),
      message.Type,
      queueName
    );
}