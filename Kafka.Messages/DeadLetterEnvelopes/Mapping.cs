
namespace Kafka.Messages;

partial class MessagesFuncs
{
  public static DeadLetterEnvelope<TKey, TValue?> ToDeadLetterEnvelope<TKey, TValue, TPayload>(
    DeadLetterMessage<TKey, TPayload> message,
    TValue? value,
    DateTime date,
    string queueName)
  =>
    CreateDeadLetterEnvelope(
      CreateKafkaMessage(
        message.MessageKey,
        value,
        SetKafkaDeadLetterMessageHeaders(
          SetKafkaHeaderCorrelationId([], message.CorrelationId).
          SetKafkaHeaderMessageId(message.MessageId).
          SetKafkaHeaderSchemaType(message.Type),
          DeserializeTopicPartitionOffset(message.Metadata),
          message.FailureReason
        ),
        date),
      queueName
    );

  public static DeadLetterEnvelope<TKey, TValue> ToDeadLetterEnvelope<TKey, TValue>(
    Envelope<TKey, TValue> envelope,
    string failureReason,
    string queueName,
    DateTime date)
  =>
    CreateDeadLetterEnvelope(
      CreateKafkaMessage(
        envelope.Key,
        envelope.Value,
        SetKafkaDeadLetterMessageHeaders(
          CopyKafkaHeaders(envelope.Metadata),
          envelope.Confirmation,
          failureReason),
        date),
      queueName);
}