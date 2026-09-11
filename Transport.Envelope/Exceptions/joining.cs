
namespace Transport.Envelope;

partial class EnvelopeFuncs
{
  static string JoinValidationErrors(IEnumerable<string> errors, string? separator = default) =>
    string.Join(separator ?? Environment.NewLine, errors);
}