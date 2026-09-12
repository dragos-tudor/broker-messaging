
using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static string? PublishingPipeline(string state, OutboundPipelineConfig config) => state switch
  {
    PipelinesTypes.Publishing => PublishingActions.Mapping,

    OutboxStates.MappingSuccess => config.UseBrokerPublisher?
      PublishingActions.Publishing:
      PublishingActions.Producing,
    OutboxStates.MappingError => PublishingActions.Abandoning,

    EnvelopeStates.PublishingSuccess => PublishingActions.Closing,
    EnvelopeStates.PublishingError => PublishingActions.Scheduling,

    EnvelopeStates.ProducingEnqueue => TerminalActions.Exit,
    EnvelopeStates.ProducingNotEnqueue => TerminalActions.Exit,
    EnvelopeStates.ProducingError => PublishingActions.Scheduling,

    OutboxStates.SchedulingExhausted => PublishingActions.Abandoning,
    OutboxStates.SchedulingNotExhausted => TerminalActions.Exit,
    OutboxStates.SchedulingError => TerminalActions.Exit,

    OutboxStates.AbandoningSuccess => TerminalActions.Exit,
    OutboxStates.AbandoningError => TerminalActions.Exit,

    OutboxStates.ClosingSuccess => TerminalActions.Exit,
    OutboxStates.ClosingError => TerminalActions.Exit,

    _ => default
  };
}
