
namespace Operations.Inbound.Inbox;

internal static class DeadLetteringStates
{
  const string Scope = $"{nameof(DeadLetteringStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
