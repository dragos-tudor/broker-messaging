
namespace Operations.Inbound.Envelope;

internal static class ConfirmingFinalStates
{
  const string Scope = $"{nameof(ConfirmingFinalStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
