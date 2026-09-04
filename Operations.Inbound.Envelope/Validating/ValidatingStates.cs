
namespace Operations.Inbound.Envelope;

static partial class EnvelopeStates
{
  internal const string ValidatingSuccess = $"{Scope}.{nameof(ValidatingSuccess)}";
  internal const string ValidatingInvalidError = $"{Scope}.{nameof(ValidatingInvalidError)}";
  internal const string ValidatingInvalidConfirmableError = $"{Scope}.{nameof(ValidatingInvalidConfirmableError)}";
  internal const string ValidatingError = $"{Scope}.{nameof(ValidatingError)}";
}
