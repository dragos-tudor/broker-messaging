using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetEphemeralDeadLetterEnvelopePipelineAction(string state) => state switch
  {
    RedirectDeadLetterEnvelopeSuccessState => EphemeralDeadLetterEnvelopeActions.Redirected,
    RedirectDeadLetterEnvelopeErrorState => EphemeralDeadLetterEnvelopeActions.CheckingRetry,

    CheckRetryDeadLetterEnvelopeExhaustedState => EphemeralDeadLetterEnvelopeActions.RetryExhausted,
    CheckRetryDeadLetterEnvelopeNotExhaustedState => EphemeralDeadLetterEnvelopeActions.RegisteringRetry,
    CheckRetryDeadLetterEnvelopeErrorState => EphemeralDeadLetterEnvelopeActions.CheckingRetry,

    RegisterRetryDeadLetterEnvelopeSuccessState => EphemeralDeadLetterEnvelopeActions.Exit,
    RegisterRetryDeadLetterEnvelopeErrorState => EphemeralDeadLetterEnvelopeActions.RegisteringRetry,

    _ => default
  };

  internal static string? GetDeadLetterEnvelopePipelineAction(string state) => state switch
  {
    PublishDeadLetterEnvelopeSuccessState => DeadLetterEnvelopeActions.Published,
    PublishDeadLetterEnvelopeErrorState => DeadLetterEnvelopeActions.Publishing,

    ProducingDeadLetterEnvelopeState => DeadLetterEnvelopeActions.Exit,
    ProduceDeadLetterEnvelopeErrorState => DeadLetterEnvelopeActions.Producing,

    _ => default
  };
}
