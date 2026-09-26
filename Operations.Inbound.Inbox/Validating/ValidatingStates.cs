
namespace Operations.Inbound.Inbox;

internal static class ValidatingStates
{
  const string Scope = $"{nameof(ValidatingStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
  internal const string InvalidError = $"{Scope}.{nameof(InvalidError)}";
}
