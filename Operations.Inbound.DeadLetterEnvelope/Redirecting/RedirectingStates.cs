
namespace Operations.Inbound.DeadLetterEnvelope;

internal static class RedirectingStates
{
  const string Scope = $"{nameof(RedirectingStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
