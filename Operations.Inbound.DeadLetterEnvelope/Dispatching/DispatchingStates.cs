
namespace Operations.Inbound.DeadLetterEnvelope;

internal static class DispatchingStates
{
  const string Scope = $"{nameof(DispatchingStates)}";
  internal const string Ack = $"{Scope}.{nameof(Ack)}";
  internal const string NotAck = $"{Scope}.{nameof(NotAck)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
