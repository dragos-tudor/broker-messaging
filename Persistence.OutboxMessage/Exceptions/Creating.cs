
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static ValidationException CreateValidationException(string error) =>
    new (error);
}