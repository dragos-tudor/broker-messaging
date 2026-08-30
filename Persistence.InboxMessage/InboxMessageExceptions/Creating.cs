
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  internal static DomainException CreateDomainException(string error) =>
    new (error);

  internal static ValidationException CreateValidationException(IEnumerable<string> errors, string? separator = default) =>
    new(JoinValidationErrors(errors, separator));

  internal static ValidationException CreateValidationException(string error) =>
    new (error);
}