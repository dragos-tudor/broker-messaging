
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static ValidationException CreateValidationException(IEnumerable<string> errors, string? separator = default) =>
    new(JoinValidationErrors(errors, separator));

  internal static ValidationException CreateValidationException(string error) =>
    new (error);
}