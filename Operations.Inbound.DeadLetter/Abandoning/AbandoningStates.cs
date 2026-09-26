
namespace Operations.Inbound.DeadLetter;

internal static class AbandoningStates
{
  const string Scope = $"{nameof(AbandoningStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
