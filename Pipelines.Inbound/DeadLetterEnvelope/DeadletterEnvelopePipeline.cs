
using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static DeadLetterEnvelopeOperation DeadLetterEnvelopePipeline(string state) => state switch
  {
    // CheckingRetryForRedirecting (side effect — pre-check gate, before Redirecting is ever attempted)
    CheckRetryDeadLetterEnvelopeExhaustedState => DeadLetterEnvelopeOperation.Exit,
    CheckRetryDeadLetterEnvelopeNotExhaustedState => DeadLetterEnvelopeOperation.Redirecting,
    CheckRetryDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.CheckingRetry,

    // Redirecting (side effect — self-loop under Router-generic budget; exhaustion invokes UpsertingRetry directly)
    RedirectDeadLetterEnvelopeSuccessState => DeadLetterEnvelopeOperation.Sending,   // non-dispatchable, see note above
    RedirectDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.Redirecting,

    // Publishing (side effect)
    PublishDeadLetterEnvelopeSuccessState => DeadLetterEnvelopeOperation.Exit,     // → Confirming, dead-letter delivered
    PublishDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.Publishing,

    // Producing (side effect — present-tense success state, 08-26 §7's exemption pattern)
    ProducingDeadLetterEnvelopeState => DeadLetterEnvelopeOperation.Deffering,     // → Deffering, dead-letter processing
    ProduceDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.Producing,

    // UpsertingRetry (side effect — shared across Redirecting/Publishing/Producing exhaustion, §4d/§9b)
    UpsertRetryDeadLetterEnvelopeSuccessState => DeadLetterEnvelopeOperation.Deffering,  // → Deffering, dead-letter processing
    UpsertRetryDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.UpsertingRetry,

    _ => DeadLetterEnvelopeOperation.Unknown
  };
}