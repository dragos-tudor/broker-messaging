
namespace Kafka.Services;

partial class ServicesFuncs
{
  public static DeadLetterEnvelope<TKey, TValue> ToDeadLetterEnvelope<TKey, TValue>(
    Envelope<TKey, TValue> envelope,
    string failureReason,
    DateTime date,
    string queueName)
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
      envelope.Type,
      failureReason,
      queueName);

  public static DeadLetterEnvelope<TKey, TValue?> ToDeadLetterEnvelope<TKey, TValue, TPayload>(
    IDeadLetterMessage<TKey, TPayload> message,
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
      message.Type,
      message.FailureReason,
      queueName
    );

}