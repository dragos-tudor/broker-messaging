
namespace Operations.Inbound.Envelope;

internal static class ConfirmingStates
{
  const string Scope = $"{nameof(ConfirmingStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}