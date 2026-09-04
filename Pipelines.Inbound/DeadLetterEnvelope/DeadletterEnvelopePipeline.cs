using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetEphemeralDeadLetterEnvelopePipelineAction(string state) => state switch
  {
    DeadLetterEnvelopeStates.RedirectingSuccess => EphemeralDeadLetterEnvelopeActions.Redirected,
    DeadLetterEnvelopeStates.RedirectingError => EphemeralDeadLetterEnvelopeActions.CheckingRetry,

    DeadLetterEnvelopeStates.CheckingRetryExhausted => EphemeralDeadLetterEnvelopeActions.RetryExhausted,
    DeadLetterEnvelopeStates.CheckingRetryNotExhausted => EphemeralDeadLetterEnvelopeActions.RegisteringRetry,
    DeadLetterEnvelopeStates.CheckingRetryError => EphemeralDeadLetterEnvelopeActions.CheckingRetry,

    DeadLetterEnvelopeStates.RegisteringRetrySuccess => TerminalActions.Exit,
    DeadLetterEnvelopeStates.RegisteringRetryError => EphemeralDeadLetterEnvelopeActions.RegisteringRetry,

    _ => default
  };

  internal static string? GetDeadLetterEnvelopePipelineAction(string state) => state switch
  {
    DeadLetterEnvelopeStates.PublishingSuccess => DeadLetterEnvelopeActions.Published,
    DeadLetterEnvelopeStates.PublishingError => DeadLetterEnvelopeActions.Publishing,

    DeadLetterEnvelopeStates.Producing => TerminalActions.Exit,
    DeadLetterEnvelopeStates.ProducingError => DeadLetterEnvelopeActions.Producing,

    _ => default
  };
}
