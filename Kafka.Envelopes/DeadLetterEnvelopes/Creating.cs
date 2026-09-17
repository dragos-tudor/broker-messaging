
namespace Kafka.Envelopes;

partial class EnvelopesFuncs
{
  internal static DeadLetterEnvelope<TKey, TValue> CreateDeadLetterEnvelope<TKey, TValue>(
    Message<TKey, TValue> message,
    string type,
    string failureReson,
    string queueName) =>
      new()
      {
        Message = message,
        Queue = queueName,
        FailureReason = failureReson,
        Type = type
      };
}