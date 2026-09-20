
using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static PersistingTransition AdvancePersistingPipeline(
    PersistingSignal signal,
    OutboundPipelineConfig config) => signal switch
  {
    PersistingEntry.Start => PersistingActions.Validating,

    ValidatingStates.Success => PersistingActions.Transacting,
    ValidatingStates.InvalidError => TerminalActions.Exit,
    ValidatingStates.Error => TerminalActions.Exit,

    TransactingStates.Success => config.PublishAfterPersist?
      OutboundPipelinesTypes.Publishing:
      TerminalActions.Exit,
    TransactingStates.Error => TerminalActions.Exit,

    _ => TerminalActions.Unknown
  };
}

