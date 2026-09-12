
using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetPublishingAction(string state, InboundPipelineConfig config) => state switch
  {
    PipelineTypes.Publishing => PublishingActions.Mapping,

    DeadLetterStates.MappingSuccess => config.UseBrokerPublisher?
      PublishingActions.Publishing:
      PublishingActions.Producing,
    DeadLetterStates.MappingError => PublishingActions.Abandoning,

    DeadLetterEnvelopeStates.PublishingSuccess => PublishingActions.Closing,
    DeadLetterEnvelopeStates.PublishingError => PublishingActions.Scheduling,

    DeadLetterEnvelopeStates.ProducingEnqueue => TerminalActions.Exit,
    DeadLetterEnvelopeStates.ProducingNotEnqueue => TerminalActions.Exit,
    DeadLetterEnvelopeStates.ProducingError => PublishingActions.Scheduling,

    DeadLetterStates.SchedulingExhausted => PublishingActions.Abandoning,
    DeadLetterStates.SchedulingNotExhausted => TerminalActions.Exit,
    DeadLetterStates.SchedulingError => TerminalActions.Exit,

    DeadLetterStates.AbandoningSuccess => TerminalActions.Exit,
    DeadLetterStates.AbandoningError => TerminalActions.Exit,

    DeadLetterStates.ClosingSuccess => TerminalActions.Exit,
    DeadLetterStates.ClosingError => TerminalActions.Exit,

    _ => default
  };
}
