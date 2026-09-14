
namespace Operations.Inbound.Envelope;

internal enum VerifyingStates
{
  VerifyingSuccess,
  VerifyingInvalidError,
  VerifyingInvalidConfirmableError,
  VerifyingError
}
