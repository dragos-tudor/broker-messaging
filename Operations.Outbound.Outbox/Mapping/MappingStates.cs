
namespace Operations.Outbound.Outbox;

internal static class MappingStates
{
  const string Scope = $"{nameof(MappingStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
