
namespace Kafka.Envelopes;

partial class EnvelopesFuncs
{
  internal static DeadLetterEnvelope<TKey, TValue> CreateDeadLetterEnvelope<TKey, TValue>(
    Message<TKey, TValue> message,
    string type,
    string queueName) =>
      new()
      {
        Message = message,
        Queue = queueName,
        Type = type
      };
}