
namespace Kafka.Envelopes;

partial class EnvelopesFuncs
{
  internal static Envelope<TKey, TValue> CreateEnvelope<TKey, TValue>(
    Message<TKey, TValue> message,
    string queueName,
    string type) =>
      new()
      {
        Message = message,
        Queue = queueName,
        Type = type
      };
}