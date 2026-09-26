
namespace Operations.Inbound.Envelope;

internal static class ConfirmingStates
{
  const string Scope = $"{nameof(ConfirmingStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}

internal static class ConfirmingFinalStates
{
  const string Scope = $"{nameof(ConfirmingFinalStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
