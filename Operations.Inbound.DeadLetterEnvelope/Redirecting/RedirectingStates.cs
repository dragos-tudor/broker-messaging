
namespace Operations.Inbound.DeadLetterEnvelope;

static partial class DeadLetterEnvelopeStates
{
  internal const string RedirectingSuccess = $"{Scope}.{nameof(RedirectingSuccess)}";
  internal const string RedirectingCircuitOpen = $"{Scope}.{nameof(RedirectingCircuitOpen)}";
  internal const string RedirectingError = $"{Scope}.{nameof(RedirectingError)}";
  internal const string RedirectingExhausted = $"{Scope}.{nameof(RedirectingExhausted)}";
}
