
namespace Operations.Inbound.DeadLetterEnvelope;

static partial class DeadLetterEnvelopeStates
{
  internal const string RedirectingSuccess = $"{Scope}.{nameof(RedirectingSuccess)}";
  internal const string RedirectingError = $"{Scope}.{nameof(RedirectingError)}";
}
