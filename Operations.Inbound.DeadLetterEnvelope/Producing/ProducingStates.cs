
namespace Operations.Inbound.DeadLetterEnvelope;

internal static class ProducingStates
{
  const string Scope = $"{nameof(ProducingStates)}";
  internal const string Enqueue = $"{Scope}.{nameof(Enqueue)}";
  internal const string NotEnqueue = $"{Scope}.{nameof(NotEnqueue)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
