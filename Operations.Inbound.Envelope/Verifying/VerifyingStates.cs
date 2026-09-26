
namespace Operations.Inbound.Envelope;

internal static class VerifyingStates
{
  const string Scope = $"{nameof(VerifyingStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string InvalidError = $"{Scope}.{nameof(InvalidError)}";
  internal const string InvalidConfirmableError = $"{Scope}.{nameof(InvalidConfirmableError)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
