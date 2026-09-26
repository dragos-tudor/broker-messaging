
namespace Operations.Inbound.Inbox;

internal static class ClosingStates
{
  const string Scope = $"{nameof(ClosingStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
