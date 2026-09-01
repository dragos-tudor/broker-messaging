
namespace Kafka.Messages;

partial class MessagesFuncs
{
  public static InboxMessage<TKey, TPayload> FromEnvelope<TKey, TValue, TPayload>(
    Envelope<TKey, TValue> envelope,
    TPayload payload)
  =>
    CreateInboxMessage(
      GetKafkaHeaderMessageId(envelope.Metadata) ?? Guid.NewGuid(),
      envelope.Key,
      payload,
      envelope.CreatedAt,
      GetKafkaHeaderCorrelationId(envelope.Metadata),
      envelope.Type ?? GetKafkaHeaderSchemaType(envelope.Metadata) ?? typeof(TPayload).Name,
      GetKafkaHeaderSchemaVersion(envelope.Metadata),
      envelope.Confirmation is not null ?
        SerializeTopicPartitionOffset(envelope.Confirmation) :
        null);

  public static Envelope<TKey, TValue> ToEnvelope<TKey, TValue, TPayload>(
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
      queueName,
      default
    );
}