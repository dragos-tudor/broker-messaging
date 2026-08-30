
namespace Operations.Inbound.DeadLetterEnvelope;

static partial class DeadLetterEnvelopeStates
{
  internal const string RedirectDeadLetterEnvelopeSuccessState = "RedirectDeadLetterEnvelopeSuccessState";
  internal const string RedirectDeadLetterEnvelopeCircuitOpenState = "RedirectDeadLetterEnvelopeCircuitOpenState";
  internal const string RedirectDeadLetterEnvelopeErrorState = "RedirectDeadLetterEnvelopeErrorState";
  internal const string RedirectDeadLetterEnvelopeExhaustedState = "RedirectDeadLetterEnvelopeExhaustedState";
}