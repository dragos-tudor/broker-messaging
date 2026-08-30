
using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static DeadLetterEnvelopeOperation DeadLetterEnvelopePipeline(string state) => state switch
  {
    // CheckingRetryForRedirecting (side effect — pre-check gate, before Redirecting is ever attempted)
    CheckRetryDeadLetterEnvelopeForRedirectingExhaustedState => DeadLetterEnvelopeOperation.Exit,
    CheckRetryDeadLetterEnvelopeForRedirectingNotExhaustedState => DeadLetterEnvelopeOperation.Redirecting,
    CheckRetryDeadLetterEnvelopeForRedirectingErrorState => DeadLetterEnvelopeOperation.CheckingRetryForRedirecting,

    // Redirecting (side effect — self-loop under Router-generic budget; exhaustion invokes UpsertingRetry directly)
    RedirectDeadLetterEnvelopeSuccessState => DeadLetterEnvelopeOperation.Sending,   // non-dispatchable, see note above
    RedirectDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.Redirecting,

    // CheckingRetryForPublishing (side effect — pre-check gate, before Publishing is ever attempted)
    CheckRetryDeadLetterEnvelopeForPublishingExhaustedState => DeadLetterEnvelopeOperation.Exit,
    CheckRetryDeadLetterEnvelopeForPublishingNotExhaustedState => DeadLetterEnvelopeOperation.Publishing,
    CheckRetryDeadLetterEnvelopeForPublishingErrorState => DeadLetterEnvelopeOperation.CheckingRetryForPublishing,

    // Publishing (side effect)
    PublishDeadLetterEnvelopeSuccessState => DeadLetterEnvelopeOperation.Exit,     // → Confirming, dead-letter delivered
    PublishDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.Publishing,

    // CheckingRetryForProducing (side effect — pre-check gate, before Producing is ever attempted)
    CheckRetryDeadLetterEnvelopeForProducingExhaustedState => DeadLetterEnvelopeOperation.Exit,
    CheckRetryDeadLetterEnvelopeForProducingNotExhaustedState => DeadLetterEnvelopeOperation.Producing,
    CheckRetryDeadLetterEnvelopeForProducingErrorState => DeadLetterEnvelopeOperation.CheckingRetryForProducing,

    // Producing (side effect — present-tense success state, 08-26 §7's exemption pattern)
    ProducingDeadLetterEnvelopeState => DeadLetterEnvelopeOperation.Exit,
    ProduceDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.Producing,

    // UpsertingRetry (side effect — shared across Redirecting/Publishing/Producing exhaustion, §4d/§9b)
    UpsertRetryDeadLetterEnvelopeSuccessState => DeadLetterEnvelopeOperation.Exit,  // → Confirming
    UpsertRetryDeadLetterEnvelopeErrorState => DeadLetterEnvelopeOperation.UpsertingRetry,

    _ => DeadLetterEnvelopeOperation.Unknown
  };
}