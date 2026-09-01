
namespace Kafka.Messages;

partial class MessagesFuncs
{
  static Guid? TryParseGuidValue(string? value)
    => Guid.TryParse(value, out var parsed) ? parsed : null;

  static int? TryParseIntValue(string? value)
    => int.TryParse(value, out var parsed) ? parsed : null;

  static long? TryParseLongValue(string? value)
    => long.TryParse(value, out var parsed) ? parsed : null;
}