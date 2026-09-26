
namespace Transport.Envelope;

partial class EnvelopeFuncs
{
  internal static ValidationException CreateValidationException(string error) =>
    new (error);
}