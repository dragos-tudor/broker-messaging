
namespace Operations.Inbound.Envelope;

static partial class EnvelopeStates
{
  internal const string VerifyingSuccess = $"{Scope}.{nameof(VerifyingSuccess)}";
  internal const string VerifyingInvalidError = $"{Scope}.{nameof(VerifyingInvalidError)}";
  internal const string VerifyingInvalidConfirmableError = $"{Scope}.{nameof(VerifyingInvalidConfirmableError)}";
  internal const string VerifyingError = $"{Scope}.{nameof(VerifyingError)}";
}
