
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static string GetDeadLetterTopicName(string topicName, string suffix = "-dlq") => $"{topicName}{suffix}";
}