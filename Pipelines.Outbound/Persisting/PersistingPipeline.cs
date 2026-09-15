
using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static PersistingContinuation GetPersistingContinuation(
    PersistingInput input,
    OutboundPipelineConfig config) => input switch
  {
    PersistingEntry.Start => PersistingActions.Validating,

    ValidatingStates.Success => PersistingActions.Transacting,
    ValidatingStates.InvalidError => TerminalActions.Exit,
    ValidatingStates.Error => TerminalActions.Exit,

    TransactingStates.Success => config.PublishAfterPersist?
      PipelinesTypes.Publishing:
      TerminalActions.Exit,
    TransactingStates.Error => TerminalActions.Exit,

    _ => TerminalActions.Unknown
  };
}
