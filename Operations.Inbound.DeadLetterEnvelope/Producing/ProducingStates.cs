
namespace Operations.Inbound.DeadLetterEnvelope;

static partial class DeadLetterEnvelopeStates
{
  internal const string Producing = $"{Scope}.{nameof(Producing)}";
  internal const string ProducingError = $"{Scope}.{nameof(ProducingError)}";
  internal const string ProducingExhausted = $"{Scope}.{nameof(ProducingExhausted)}";
}
