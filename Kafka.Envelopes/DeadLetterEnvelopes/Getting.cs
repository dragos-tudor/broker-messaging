
namespace Kafka.Envelopes;

partial class EnvelopesFuncs
{
  internal static string GetDeadLetterEnvelopeTopicName(string topicName, string suffix = "-dlq") =>
    $"{topicName}{suffix}";
}