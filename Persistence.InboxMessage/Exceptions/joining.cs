
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  static string JoinValidationErrors(IEnumerable<string> errors, string? separator = default) =>
    string.Join(separator ?? Environment.NewLine, errors);
}