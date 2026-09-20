
using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static PublishingTransition AdvancePublishingPipeline(
    PublishingSignal signal,
    OutboundPipelineConfig config) => signal switch
  {
    PublishingEntry.Start => PublishingActions.Mapping,

    MappingStates.Success => config.UseBrokerPublisher?
      PublishingActions.Publishing:
      PublishingActions.Producing,
    MappingStates.Error => PublishingActions.Abandoning,

    PublishingStates.Success => PublishingActions.Closing,
    PublishingStates.Error => PublishingActions.Scheduling,

    ProducingStates.Enqueue => TerminalActions.Exit,
    ProducingStates.NotEnqueue => TerminalActions.Exit,
    ProducingStates.Error => PublishingActions.Scheduling,

    SchedulingStates.Exhausted => PublishingActions.Abandoning,
    SchedulingStates.NotExhausted => TerminalActions.Exit,
    SchedulingStates.Error => TerminalActions.Exit,

    AbandoningStates.Success => TerminalActions.Exit,
    AbandoningStates.Error => TerminalActions.Exit,

    ClosingStates.Success => TerminalActions.Exit,
    ClosingStates.Error => TerminalActions.Exit,

    _ => TerminalActions.Unknown
  };
}

