
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  internal static ValidationException CreateValidationException(string error) =>
    new (error);
}