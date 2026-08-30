namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  internal static int ParseIntValue(string? value, int fallback)
    => int.TryParse(value, out var parsed) ? parsed : fallback;

  internal static long ParseLongValue(string? value, long fallback)
    => long.TryParse(value, out var parsed) ? parsed : fallback;

  internal static double ParseDoubleValue(string? value, double fallback)
    => double.TryParse(value, out var parsed) ? parsed : fallback;
}