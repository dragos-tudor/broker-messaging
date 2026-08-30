
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  static string JoinValidationErrors(IEnumerable<string> errors, string? separator = default) =>
    string.Join(separator ?? Environment.NewLine, errors);
}