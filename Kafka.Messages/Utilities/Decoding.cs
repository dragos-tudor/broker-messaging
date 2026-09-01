
namespace Kafka.Messages;

partial class MessagesFuncs
{
  static string? DecodeString(byte[]? value) =>
    value is not null ? Encoding.UTF8.GetString(value) : default;
}