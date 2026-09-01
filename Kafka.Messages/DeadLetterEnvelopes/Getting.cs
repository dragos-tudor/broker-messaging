
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static string GetDeadLetterEnvelopeTopicName(string topicName, string suffix = "-dlq") =>
    $"{topicName}{suffix}";
}