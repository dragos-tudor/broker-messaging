
namespace Operations.Inbound.Envelope;

internal static class ConvertingStates
{
  const string Scope = $"{nameof(ConvertingStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Invalid = $"{Scope}.{nameof(Invalid)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
