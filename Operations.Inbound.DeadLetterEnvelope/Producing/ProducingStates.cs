
namespace Operations.Inbound.DeadLetterEnvelope;

static partial class DeadLetterEnvelopeStates
{
  const string Scope = "DeadLetterEnvelope";
  internal const string ProducingEnqueue = $"{Scope}.{nameof(ProducingEnqueue)}";
  internal const string ProducingNotEnqueue = $"{Scope}.{nameof(ProducingNotEnqueue)}";
  internal const string ProducingError = $"{Scope}.{nameof(ProducingError)}";
}
