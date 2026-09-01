
namespace Kafka.Messages;

partial class MessagesFuncs
{
  static byte[]? EncodeString(string? value) =>
    value is not null ? Encoding.UTF8.GetBytes(value) : default;
}