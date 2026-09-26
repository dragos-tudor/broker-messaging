
namespace Operations.Inbound.Envelope;

internal static class CapturingStates
{
  const string Scope = $"{nameof(CapturingStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string NotCaptured = $"{Scope}.{nameof(NotCaptured)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
