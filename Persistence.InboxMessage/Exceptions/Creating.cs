
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  internal static ValidationException CreateValidationException(string error) =>
    new (error);
}