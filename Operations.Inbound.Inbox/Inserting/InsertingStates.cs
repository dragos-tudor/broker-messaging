
namespace Operations.Inbound.Inbox;

internal static class InsertingStates
{
  const string Scope = $"{nameof(InsertingStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
  internal const string Idempotent = $"{Scope}.{nameof(Idempotent)}";
}
