
namespace Operations.Inbound.Inbox;

internal static class HandlingStates
{
  const string Scope = $"{nameof(HandlingStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string DomainError = $"{Scope}.{nameof(DomainError)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
