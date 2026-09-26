
namespace Operations.Outbound.Outbox;

internal static class TransactingStates
{
  const string Scope = $"{nameof(TransactingStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
